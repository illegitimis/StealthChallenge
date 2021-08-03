namespace StealthChallenge.ConsoleClient
{
    using CommandLine;
    using System;
    using System.Collections.Generic;

    public static class ConsoleWriter
    {
        public static void Error(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ResetColor();
        }

        public static void Warning(string warning)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(warning);
            Console.ResetColor();
        }

        public static void Info(string info)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(info);
            Console.ResetColor();
        }

        public static void WriteErrors(IEnumerable<Error> errors)
        {
            foreach (var err in errors)
                ConsoleWriter.Error(err.ToString());
        }
    }

}
