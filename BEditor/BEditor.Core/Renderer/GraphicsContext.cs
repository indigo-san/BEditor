﻿
using OpenTK;

namespace BEditor.Core.Renderer
{
    public sealed class GraphicsContext : BaseGraphicsContext
    {
        private readonly GameWindow GameWindow;

        public GraphicsContext(int width, int height) : base(width, height)
        {
            GameWindow = new GameWindow(width, height);

            Initialize();
        }

        public override void MakeCurrent() => GameWindow.MakeCurrent();

        public override void SwapBuffers() => GameWindow.SwapBuffers();

        public override void Resize(int width, int height, bool Perspective = false, float x = 0, float y = 0, float z = 1024, float tx = 0, float ty = 0, float tz = 0, float near = 0.1F, float far = 20000)
        {
            base.Resize(width, height, Perspective, x, y, z, tx, ty, tz, near, far);
            GameWindow.Size = new System.Drawing.Size(width, height);
        }

        public override void Dispose()
        {
            base.Dispose();
            GameWindow.Dispose();
        }
    }
}
