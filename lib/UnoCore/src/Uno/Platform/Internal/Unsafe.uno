using Uno.Compiler.ExportTargetInterop;

namespace Uno.Platform.Internal
{
    // For code that should never be used in production but is useful for internal work
    static public class Unsafe
    {
        //--------------------------------------------------
        // QUIT
        //
        // Not allowed on android or ios, but possible of course
        // We have tests that we rely on being able to kill like this
        public static extern(!Android && !iOS) void Quit()
        {
            if (CoreApp.Current is Application)
                Application.Current.Window.Close();
            else
                debug_log "\n\n-- DIDNT QUIT, fix this --\n\n"; // {TODO}
        }

        [Foreign(Language.ObjC)]
        public static extern(iOS) void Quit()
        @{
            exit(0);
        @}

        [Foreign(Language.Java)]
        public static extern(Android) void Quit()
        @{
            com.fuse.App.TerminateNow();
        @}

        //--------------------------------------------------
        // Thing you are adding
        //
        // Justify why it's needed
    }
}
