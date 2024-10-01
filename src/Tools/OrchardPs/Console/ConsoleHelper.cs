using System;
using System.Drawing;

namespace OrchardPs.Console {
    internal static class ConsoleHelper {
        private static readonly ColorConverter _colorConverter = new ColorConverter();

        public static void WriteToConsole(string text, ConsoleColor? color) {
            var defaultColor = default(ConsoleColor);

            if (color != null) {
                defaultColor = System.Console.ForegroundColor;
                System.Console.ForegroundColor = color.Value;
            }

            System.Console.Write(text);

            if (color != null) {
                System.Console.ForegroundColor = defaultColor;
            }
        }

        public static void WriteToConsole(string text, string color) {
            var defaultColor = default(ConsoleColor);

            if (color != null) {
                defaultColor = System.Console.ForegroundColor;

                // ReSharper disable once PossibleNullReferenceException
                System.Console.ForegroundColor = ClosestConsoleColor((Color) _colorConverter.ConvertFromString(color));
            }

            System.Console.Write(text);

            if (color != null) {
                System.Console.ForegroundColor = defaultColor;
            }
        }

        /// <remarks>
        /// Based on: http://stackoverflow.com/questions/1988833/converting-color-to-consolecolor
        /// </remarks>>
        private static ConsoleColor ClosestConsoleColor(Color color) {
            ConsoleColor result = 0;
            double delta = double.MaxValue;
            foreach (ConsoleColor consoleColor in Enum.GetValues(typeof (ConsoleColor))) {
                string name = Enum.GetName(typeof (ConsoleColor), consoleColor);

                // ReSharper disable once AssignNullToNotNullAttribute
                Color c = Color.FromName(name);

                double t = Math.Pow(c.R - (double) color.R, 2.0)
                           + Math.Pow(c.G - (double) color.G, 2.0)
                           + Math.Pow(c.B - (double) color.B, 2.0);

                if (t < delta) {
                    delta = t;
                    result = consoleColor;
                }
            }

            return result;
        }
    }
}