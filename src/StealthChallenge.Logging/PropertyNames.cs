namespace StealthChallenge.Logging
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Log event property names.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class PropertyNames
    {
        public const string ClassName = "cls";
        public const string MemberName = "fct";
        public const string FilePath = "pth";
        public const string LineNumber = "lno";

        public const string Message = "@m";
        public const string Template = "@mt";
        public const string Level = "@l";
        public const string Timestamp = "@t";

        public const string DestructuredType = "$type";
        public const string ExceptionMessage = "@x";
        public const string ExceptionDetail = "ExceptionDetail";
        public const string ApplicationName = "app";

        public const string ControllerName = "ctrl";
        public const string ActionName = "actn";

        public const string CommunicationMessageId = "messageId";
        public const string DeviceId = "deviceId";
        public const string CarId = "carId";
        public const string CommandId = "commandid";

        // Timings related properties
        public const string Outcome = "Outcome";
        public const string Elapsed = "Elapsed";
        public const string OperationId = "OperationId";

        public const string UsageTarget = "Target";
        public const string SourceContext = nameof(SourceContext);

        public const string DestructuredCommand = "{@command}";
    }
}