namespace Android
{
    using Uno;
    using Uno.Compiler.ExportTargetInterop;

    public extern(ANDROID) static class Resources {

        [Foreign(Language.Java)]
        public static extern int GetID(string path)
        @{
            return com.fuse.R.get(path);
        @}
    }
}
