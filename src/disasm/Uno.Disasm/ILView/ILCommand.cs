namespace Uno.Disasm.ILView
{
    public abstract class ILCommand
    {
        public abstract string Header { get; }

        public virtual ILIcon GetIcon(ILItem item)
        {
            return 0;
        }

        public virtual bool IsDefault(ILItem item)
        {
            return false;
        }

        public virtual bool CanShow(ILItem item)
        {
            return true;
        }

        public virtual bool CanExecute(ILItem item)
        {
            return CanShow(item);
        }

        public abstract void Execute(ILItem item);
    }
}
