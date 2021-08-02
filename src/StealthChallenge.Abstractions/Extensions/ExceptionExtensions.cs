namespace StealthChallenge.Abstractions.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public static class ExceptionExtensions
    {
        public static Exception Reduce(this Exception exception)
        {
            if (exception.InnerException == null) return exception;

            var ex = new Exception(nameof(Reduce));

            if (exception is AggregateException agex)
            {
                Decorate(ex, agex);
                foreach (var exSource in agex.InnerExceptions)
                {
                    Decorate(ex, exSource);
                }
            }
            else
            {
                Decorate(ex, exception);

                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                    Decorate(ex, exception);
                }
            }

            return ex;
        }

        public static Dictionary<string, object> ToDictionary(this Exception exception)
        {
            var ex = exception.Reduce();
            var dict = new Dictionary<string, object>
            {
                ["name"] = ex.GetType().Name,
                ["message"] = ex.Message
            };
            foreach (DictionaryEntry de in ex.Data ?? new Dictionary<string, object>())
            {
                dict.Add(de.Key.ToString(), de.Value);
            }
            return dict;
        }

        public static string Description(this Exception ex) =>
            $"{ex.GetType().Name}: {ex.Message}";

        private static void Decorate(Exception exDestination, Exception exSource)
        {
            exDestination.Data.Add(exSource.GetType().Name, exSource.Message);

            foreach (DictionaryEntry de in exSource.Data)
            {
                exDestination.Data.Add(de.Key, de.Value);
            }
        }
    }
}