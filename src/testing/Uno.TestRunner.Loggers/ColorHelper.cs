using System;

namespace Uno.TestRunner.Loggers
{
    public static class ColorHelper
    {
        public static ConsoleColor DefaultBackgroundColor { get; }
        public static ConsoleColor DefaultForegroundColor { get; }

        static ColorHelper()
        {
            DefaultBackgroundColor = Console.BackgroundColor;
            DefaultForegroundColor = Console.ForegroundColor;
        }

        public static void SetDefault()
        {
            Set(DefaultForegroundColor, DefaultBackgroundColor);
        }

        public static void Set(ConsoleColor foreground, ConsoleColor background)
        {
            SetForeground(foreground);
            SetBackground(background);
        }

        public static void SetBackground(ConsoleColor background)
        {
            Console.BackgroundColor = background;
        }

        public static void SetForeground(ConsoleColor foreground)
        {
            Console.ForegroundColor = foreground;
        }
    }
}