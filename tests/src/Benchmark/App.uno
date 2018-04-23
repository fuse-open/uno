using Uno;
using Uno.Diagnostics;

class App : Application
{
    public App()
    {
        //Run("BinaryTrees", BinaryTrees.Main);
        Run("FannkuchRedux", FannkuchRedux.Main);
        Run("Fasta", Fasta.Main);
        Run("FastaReduxTest", FastaReduxTest.Main);
        Run("NBody", NBody.Main);
    }

    void Run(string name, Action action)
    {
        double start = Clock.GetSeconds();
        action();
        double end = Clock.GetSeconds();
        debug_log name + ": " + (end - start) * 1000.0;
    }
}
