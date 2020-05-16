using System;
using Uno.Disasm.ILView;

namespace Uno.Disasm
{
    public interface IILView
    {
        void BeginInvoke<T>(Action<T> method, T arg);
        bool TryOpenProject(out string filename);
        void OnProjectChanged(string project);
        void OnBuildStarting(BuildItem build);
        void OnBuildFinished(BuildItem build);
    }
}