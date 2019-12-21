using System;
using OpenGL;
using Uno.Diagnostics;
using Uno.Graphics;
using Uno.Platform;
using Uno.Runtime.Implementation;

namespace Uno
{
    public interface IAppHost
    {
        WindowBackend GetWindowBackend();
        GraphicsContextHandle GetGraphicsContext();
    }

    public static class ApplicationContext
    {
        public static IAppHost AppHost { get; private set; }

        public static void Initialize(IAppHost appHost)
        {
            AppHost = appHost;
        }
    }
}
