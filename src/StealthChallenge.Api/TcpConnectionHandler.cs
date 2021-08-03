using Microsoft.AspNetCore.Connections;
using StealthChallenge.Abstractions.Infrastructure.Commands;
using System;
using StealthChallenge.Abstractions.Logging;
using System.Linq;
using System.Threading.Tasks;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Buffers;
using StealthChallenge.Abstractions.Domain.Services;
using StealthChallenge.Abstractions.Infrastructure.Services;
using StealthChallenge.MessagePack;

namespace StealthChallenge.Api
{


    public class TcpConnectionHandler : ConnectionHandler
    {
        private readonly ISerializeClientCommands _serializer;
        private readonly ILogger<TcpConnectionHandler> _logger;
        private readonly IClientCommandStrategy _clientCommandStrategy;
        private readonly IManageRunningGames _gm;

        public TcpConnectionHandler(
            ISerializeClientCommands serializer,
            ILoggerFactory loggerFactory,
            IClientCommandStrategy clientCommandStrategy,
            IManageRunningGames gm)
        {
            _serializer = serializer;
            _logger = loggerFactory.Get<TcpConnectionHandler>();
            _clientCommandStrategy = clientCommandStrategy;
            _gm = gm;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            try
            {
                // client closing TCP connection triggers this cancellation token 
                while (!connection.ConnectionClosed.IsCancellationRequested)
                {
                    ReadResult result = await connection.Transport.Input
                        .ReadAsync(connection.ConnectionClosed)
                        .ConfigureAwait(false);

                    var buffer = result.Buffer;
                    _logger.Debug(MtReadInput, new object[] { buffer.Length, result.IsCanceled, result.IsCompleted });

                    if (result.IsCompleted)
                    {
                        _logger.Debug(MtEos, new object[] { connection.ConnectionId });
                        break;
                    }

                    if (buffer.IsEmpty)
                    {
                        _logger.Warn(MtEmptyBuffer, connection.ConnectionId);
                        continue;
                    }

                    var clientCommand = _serializer.Deserialize(buffer.ToArray()) as AbstractCommand;
                    await _gm.AddConnectionAsync(clientCommand.User, connection.ConnectionId, connection.Transport.Output);
                    await _clientCommandStrategy
                        .HandleAsync(clientCommand, connection.Transport.Output)
                        .ConfigureAwait(false);

                    // Moves forward the pipeline's read cursor to after the consumed data.
                    // Marks the extent of the data that has been successfully processed.
                    connection.Transport.Input.AdvanceTo(buffer.End);
                }
            }
            catch (ConnectionResetException)
            {
                /* Connection reset by peer. Reason: Reading from pipe failed */
            }
            catch (OperationCanceledException ocex) when (ocex.CancellationToken.IsCancellationRequested)
            {
                /*
                 * TCP sender initiates cancellation
                 * Caller: Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure.KestrelConnection.ExecuteAsync()
                 * Message: The operation was canceled.
                 */
            }
            catch (ObjectDisposedException)
            {
                /* case of InvalidOperationException */
            }
            catch (SocketException socketTimeOutEx) when (socketTimeOutEx.SocketErrorCode == SocketError.TimedOut)
            {
                /*
                 * Message: Connection timed out
                 * ErrorCode: 110
                 * Bottom stack trace: System.IO.Pipelines.Pipe.GetReadResult(ReadResult& result)
                 */
            }
            catch (ConnectionAbortedException)
            {
                /* Message: The connection was aborted because the server is shutting down.*/
            }
            catch (ArgumentOutOfRangeException argOutOfRangeEx) when (argOutOfRangeEx.Message == "Specified argument was out of the range of valid values.")
            {
                /* Thrown when the pipe is completed by the device is still sending data
                at System.IO.Pipelines.BufferSegment.set_End(Int32 value)
                In the logs you can see "closed gracefully" and sending FIN */
            }
            catch (Exception ex)
            {
                /* catch and log remaining connection faults */
                ex.Data.Add("Tcp", connection.ConnectionId);
                _logger.Error(ex);
            }
            finally
            {
                _logger.Debug(MtDisconnect, new[] { connection.ConnectionId });

                await _gm
                    .DisconnectAsync(connection.ConnectionId)
                    .ConfigureAwait(false);
            }
        }

        private const string RemoteEndpointMessageTemplate = "Remote endpoint {@remoteEndPoint} not an ipendpoint";
        private const string LocalEndpointMessageTemplate = "Local endpoint {@localEndPoint} not an ipendpoint";
        private const string TcpConnected = "Connection {connectionId} connected. Remote address {ra} port {rp}. Local address {la} port {lp}";
        private const string MtReadInput = "Input transport pipe read result length {length}. Cancelled {cancel}. Completed {complete}";
        private const string MtDisconnect = "Connection {connectionId} disconnected";
        private const string MtEos = "End of the data stream has been reached for {connectionId}";
        private const string MtEmptyBuffer = "Buffer was empty for {connectionId}";
    }
}
