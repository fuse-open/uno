namespace Uno.Platform
{
    public class FrameChangedEventArgs : EventArgs
    {
        public FrameChangedEventArgs(Rect newFrame)
        {
            NewFrame = newFrame;
        }

        public Rect NewFrame { get; private set; }
    }
}