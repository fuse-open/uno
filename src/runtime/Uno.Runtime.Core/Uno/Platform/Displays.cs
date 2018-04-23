// This file was generated based on Library/Core/UnoCore/Source/Uno/Platform/Displays.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Platform
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public static class Displays
    {
        static Displays()
        {
            Displays.All = new global::System.Collections.Generic.List<Display>();
            Displays.PopulateDisplaysList();
        }

        public static void PopulateDisplaysList()
        {
            Displays.All.Add(new DesktopDisplay());
        }

        public static Display GetMainDisplay()
        {
            return Displays.All[0];
        }

        public static void TickAll(TimerEventArgs args)
        {
            global::System.Collections.Generic.List<Display>.Enumerator enum1 = Displays.All.GetEnumerator();

            try
            {
                while (enum1.MoveNext())
                {
                    Display d = enum1.Current;
                    d.OnTick(args);
                }
            }
            finally
            {
                enum1.Dispose();
            }
        }

        public static global::System.Collections.Generic.List<Display> All
        {
            get;
            set;
        }

        public static Display MainDisplay
        {
            get { return Displays.GetMainDisplay(); }
        }
    }
}
