namespace Uno.AppLoader
{
    class DummyUnoApp : Uno.Application
    {
        public override void Draw()
        {
            Uno.Float4 cc;
            var frameTime = Uno.Diagnostics.Clock.GetSeconds();
            cc.X = (float)Math.Sin(frameTime) * 0.5f + 0.5f;
            cc.Y = (float)Math.Cos(frameTime) * 0.5f + 0.5f;
            cc.Z = cc.X + cc.Y;
            cc.W = 1;
            GraphicsController.ClearColor = cc;
        }
    }
}
