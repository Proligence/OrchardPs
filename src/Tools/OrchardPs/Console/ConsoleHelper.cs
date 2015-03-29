namespace OrchardPs.Console
{
    using System;
    using System.Drawing;

    internal static class ConsoleHelper
    {
        private static readonly ColorConverter ColorConverter = new ColorConverter();

        public static void WriteToConsole(string text, ConsoleColor? color)
        {
            ConsoleColor defaultColor = default(ConsoleColor);

            if (color != null)
            {
                defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = color.Value;
            }

            Console.Write(text);

            if (color != null)
            {
                Console.ForegroundColor = defaultColor;
            }
        }

        public static void WriteToConsole(string text, string color)
        {
            ConsoleColor defaultColor = default(ConsoleColor);

            if (color != null)
            {
                defaultColor = Console.ForegroundColor;

                // ReSharper disable once PossibleNullReferenceException
                Console.ForegroundColor = ClosestConsoleColor((Color)ColorConverter.ConvertFromString(color));
            }

            Console.Write(text);

            if (color != null)
            {
                Console.ForegroundColor = defaultColor;
            }
        }

        /// <remarks>
        /// Based on: http://stackoverflow.com/questions/1988833/converting-color-to-consolecolor
        /// </remarks>>
        private static ConsoleColor ClosestConsoleColor(Color color)
        {
            ConsoleColor result = 0;
            double delta = double.MaxValue;
            foreach (ConsoleColor consoleColor in Enum.GetValues(typeof(ConsoleColor)))
            {
                string name = Enum.GetName(typeof(ConsoleColor), consoleColor);

                // ReSharper disable once AssignNullToNotNullAttribute
                Color c = Color.FromName(name);

                double t = Math.Pow(c.R - (double)color.R, 2.0)
                           + Math.Pow(c.G - (double)color.G, 2.0)
                           + Math.Pow(c.B - (double)color.B, 2.0);

                if (t < delta)
                {
                    delta = t;
                    result = consoleColor;
                }
            }

            return result;
        }
    }
}