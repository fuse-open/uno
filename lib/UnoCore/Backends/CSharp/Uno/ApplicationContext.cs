using System;
using OpenGL;
using Uno.Diagnostics;
using Uno.Graphics;
using Uno.Platform;

namespace Uno
{
    public interface IAppHost
    {
        WindowBackend GetWindowBackend();
        GraphicsContextBackend GetGraphicsContextBackend();
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
