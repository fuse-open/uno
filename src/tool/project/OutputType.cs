namespace Uno.ProjectFormat
{
    public enum OutputType
    {
        Undefined,
        App,
        /** Test project that needs a graphics context (etc) to run */
        AppTest,
        Console,
        Library,
        Test,
    }
}
