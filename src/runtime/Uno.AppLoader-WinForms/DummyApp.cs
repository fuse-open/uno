using Uno.Diagnostics;

namespace Uno.AppLoader
{
    class DummyApp : Application
    {
        public override void Draw()
        {
            Float4 cc;
            var frameTime = Clock.GetSeconds();
            cc.X = (float)Math.Sin(frameTime) * 0.5f + 0.5f;
            cc.Y = (float)Math.Cos(frameTime) * 0.5f + 0.5f;
            cc.Z = cc.X + cc.Y;
            cc.W = 1;
            GraphicsController.ClearColor = cc;
        }
    }
}
