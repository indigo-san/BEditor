﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using BEditor.Drawing;
using BEditor.Drawing.Pixel;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace BEditor.Graphics
{

    public unsafe sealed class GraphicsContext : IDisposable
    {
        private static bool isFirst = true;
        private readonly Window* _window;
        private readonly Shader _textureShader;
        private readonly Shader _shader;
        private readonly Shader _lightShader;
        private readonly Shader _texLightShader;
        private readonly Shader _lineShader;
        private readonly SynchronizationContext _synchronization;

        public GraphicsContext(int width, int height)
        {
            Width = width;
            Height = height;
            _synchronization = AsyncOperationManager.SynchronizationContext;

            if (isFirst)
            {
                GLFW.Init();
                Tool.ThrowGLFWError();
            }

            GLFW.WindowHint(WindowHintClientApi.ClientApi, ClientApi.OpenGlApi);
            GLFW.WindowHint(WindowHintInt.ContextVersionMajor, 3);
            GLFW.WindowHint(WindowHintInt.ContextVersionMinor, 3);
            GLFW.WindowHint(WindowHintBool.Visible, false);
            _window = GLFW.CreateWindow(width, height, "", null, null);
            GLFW.SetWindowSizeLimits(_window, width, height, width, height);
            Tool.ThrowGLFWError();
            MakeCurrent();

            if (isFirst)
            {
                var context = new GLFWBindingsContext();
                GL.LoadBindings(context);
                OpenTK.Graphics.OpenGL.GL.LoadBindings(context);
                OpenTK.Graphics.ES11.GL.LoadBindings(context);
                OpenTK.Graphics.ES20.GL.LoadBindings(context);
                OpenTK.Graphics.ES30.GL.LoadBindings(context);

                isFirst = false;
            }

            _textureShader = ShaderFactory.Texture.Create();
            _shader = ShaderFactory.Default.Create();
            _lightShader = ShaderFactory.Lighting.Create();
            _texLightShader = ShaderFactory.TextureLighting.Create();
            _lineShader = ShaderFactory.Line.Create();


            Camera = new OrthographicCamera(new(0, 0, 1024), width, height);

            Tool.ThrowGLError();

            // カラーバッファ用のテクスチャを用意する
            ColorRenderbuffer = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, ColorRenderbuffer);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // デプスバッファ用のレンダーバッファを用意する
            DepthRenderbuffer = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthRenderbuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, Width, Height);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

            // フレームバッファオブジェクトを作成する
            Framebuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Framebuffer);

            // フレームバッファオブジェクトにカラーバッファとしてテクスチャを結合する
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, ColorRenderbuffer, 0);

            // フレームバッファオブジェクトにデプスバッファとしてレンダーバッファを結合する
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, DepthRenderbuffer);

            // フレームバッファオブジェクトの結合を解除する
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);


            Tool.ThrowGLError();

            Clear();
        }
        ~GraphicsContext()
        {
            if (!IsDisposed) Dispose();
        }

        public GraphicsHandle ColorRenderbuffer { get; }
        public GraphicsHandle DepthRenderbuffer { get; }
        public GraphicsHandle Framebuffer { get; }
        public int Width { get; }
        public int Height { get; }
        public float Aspect => ((float)Width) / ((float)Height);
        public bool IsCurrent => GLFW.GetCurrentContext() == _window;
        public bool IsDisposed { get; private set; }
        public Camera Camera { get; set; }
        public Light? Light { get; set; }

        public void Clear()
        {
            MakeCurrent();

            GL.Viewport(0, 0, Width, Height);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Framebuffer);

            // アンチエイリアス
            GL.Enable(EnableCap.LineSmooth);
            GL.Enable(EnableCap.PolygonSmooth);

            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
            GL.Hint(HintTarget.TextureCompressionHint, HintMode.Nicest);
            Tool.ThrowGLError();

            GL.Disable(EnableCap.DepthTest);
            Tool.ThrowGLError();

            GL.ClearColor(default);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Tool.ThrowGLError();
        }
        private void MakeCurrent()
        {
            if (!IsCurrent)
            {
                GLFW.MakeContextCurrent(_window);
                Tool.ThrowGLFWError();
            }
        }
        public void DrawTexture(Texture texture)
        {
            MakeCurrent();
            texture.Use(TextureUnit.Texture0);

            _textureShader.Use();

            var vertexLocation = _textureShader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _textureShader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            _textureShader.SetInt("texture", 0);

            GL.Enable(EnableCap.Blend);


            GL.BlendEquationSeparate(BlendEquationMode.FuncAdd, BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            _textureShader.SetVector4("color", texture.Color.ToVector4());
            _textureShader.SetMatrix4("model", texture.Transform.Matrix);
            _textureShader.SetMatrix4("view", Camera.GetViewMatrix());
            _textureShader.SetMatrix4("projection", Camera.GetProjectionMatrix());

            _textureShader.Use();

            texture.Draw(TextureUnit.Texture0);

            Tool.ThrowGLError();
        }
        public void DrawTexture(Texture texture, Action blend)
        {
            if (Light is null)
            {
                MakeCurrent();

                texture.Use(TextureUnit.Texture0);

                _textureShader.Use();

                var vertexLocation = _textureShader.GetAttribLocation("aPosition");
                GL.EnableVertexAttribArray(vertexLocation);
                GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

                var texCoordLocation = _textureShader.GetAttribLocation("aTexCoord");
                GL.EnableVertexAttribArray(texCoordLocation);
                GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

                _textureShader.SetInt("texture", 0);

                GL.Enable(EnableCap.Blend);

                blend();


                _textureShader.SetVector4("color", texture.Color.ToVector4());
                _textureShader.SetMatrix4("model", texture.Transform.Matrix);
                _textureShader.SetMatrix4("view", Camera.GetViewMatrix());
                _textureShader.SetMatrix4("projection", Camera.GetProjectionMatrix());

                _textureShader.Use();

                texture.Draw(TextureUnit.Texture0);

                Tool.ThrowGLError();
            }
            else
            {
                DrawTextureWithLight(texture, blend);
            }
        }
        private void DrawTextureWithLight(Texture texture, Action blend)
        {
            MakeCurrent();

            texture.Use(TextureUnit.Texture0);

            _texLightShader.Use();

            var vertexLocation = _texLightShader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _texLightShader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            var normalLocation = _texLightShader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            GL.BindVertexArray(texture.VertexArrayObject);

            _texLightShader.SetInt("texture0", 0);

            GL.Enable(EnableCap.Blend);

            blend();

            //InvalidEnum
            //GL.Enable(EnableCap.Texture2D);

            _texLightShader.SetMatrix4("model", texture.Transform.Matrix);
            _texLightShader.SetMatrix4("view", Camera.GetViewMatrix());
            _texLightShader.SetMatrix4("projection", Camera.GetProjectionMatrix());
            _texLightShader.SetVector3("viewPos", Camera.Position);
            _texLightShader.SetVector4("color", texture.Color.ToVector4());

            _texLightShader.SetVector4("material.ambient", texture.Material.Ambient.ToVector4());
            _texLightShader.SetVector4("material.diffuse", texture.Material.Diffuse.ToVector4());
            _texLightShader.SetVector4("material.specular", texture.Material.Specular.ToVector4());
            _texLightShader.SetFloat("material.shininess", texture.Material.Shininess);

            _texLightShader.SetVector3("light.position", Light!.Position);
            _texLightShader.SetVector4("light.ambient", Light.Ambient.ToVector4());
            _texLightShader.SetVector4("light.diffuse", Light.Diffuse.ToVector4());
            _texLightShader.SetVector4("light.specular", Light.Specular.ToVector4());

            _texLightShader.Use();

            texture.Draw(TextureUnit.Texture0);

            Tool.ThrowGLError();
        }
        public void DrawCube(Cube cube)
        {
            MakeCurrent();

            if (Light is null)
            {
                _shader.Use();

                var vertexLocation = _shader.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(vertexLocation);
                GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);


                GL.BindVertexArray(cube.VertexArrayObject);

                _shader.SetMatrix4("model", cube.Transform.Matrix);
                _shader.SetMatrix4("view", Camera.GetViewMatrix());
                _shader.SetMatrix4("projection", Camera.GetProjectionMatrix());
                _shader.SetVector4("color", cube.Color.ToVector4());

                cube.Draw();

                Tool.ThrowGLError();
            }
            else
            {
                DrawCubeWithLight(cube);
            }
        }
        private void DrawCubeWithLight(Cube cube)
        {
            _lightShader.Use();

            var vertexLocation = _lightShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            var normalLocation = _lightShader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

            GL.BindVertexArray(cube.VertexArrayObject);

            _lightShader.SetMatrix4("model", cube.Transform.Matrix);
            _lightShader.SetMatrix4("view", Camera.GetViewMatrix());
            _lightShader.SetMatrix4("projection", Camera.GetProjectionMatrix());
            _lightShader.SetVector3("viewPos", Camera.Position);
            _lightShader.SetVector4("color", cube.Color.ToVector4());

            _lightShader.SetVector4("material.ambient", cube.Material.Ambient.ToVector4());
            _lightShader.SetVector4("material.diffuse", cube.Material.Diffuse.ToVector4());
            _lightShader.SetVector4("material.specular", cube.Material.Specular.ToVector4());
            _lightShader.SetFloat("material.shininess", cube.Material.Shininess);

            _lightShader.SetVector3("light.position", Light!.Position);
            _lightShader.SetVector4("light.ambient", Light.Ambient.ToVector4());
            _lightShader.SetVector4("light.diffuse", Light.Diffuse.ToVector4());
            _lightShader.SetVector4("light.specular", Light.Specular.ToVector4());


            cube.Draw();

            Tool.ThrowGLError();
        }
        public void DrawBall(Ball ball)
        {
            MakeCurrent();

            if (Light is null)
            {
                _shader.Use();

                var vertexLocation = _shader.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(vertexLocation);
                GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);


                GL.BindVertexArray(ball.VertexArrayObject);

                _shader.SetMatrix4("model", ball.Transform.Matrix);
                _shader.SetMatrix4("view", Camera.GetViewMatrix());
                _shader.SetMatrix4("projection", Camera.GetProjectionMatrix());
                _shader.SetVector4("color", ball.Color.ToVector4());

                ball.Draw();

                Tool.ThrowGLError();
            }
            else
            {
                DrawBallWithLight(ball);
            }
        }
        private void DrawBallWithLight(Ball ball)
        {
            _lightShader.Use();

            var vertexLocation = _lightShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            var normalLocation = _lightShader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

            GL.BindVertexArray(ball.VertexArrayObject);

            _lightShader.SetMatrix4("model", ball.Transform.Matrix);
            _lightShader.SetMatrix4("view", Camera.GetViewMatrix());
            _lightShader.SetMatrix4("projection", Camera.GetProjectionMatrix());
            _lightShader.SetVector3("viewPos", Camera.Position);
            _lightShader.SetVector4("color", ball.Color.ToVector4());

            _lightShader.SetVector4("material.ambient", ball.Material.Ambient.ToVector4());
            _lightShader.SetVector4("material.diffuse", ball.Material.Diffuse.ToVector4());
            _lightShader.SetVector4("material.specular", ball.Material.Specular.ToVector4());
            _lightShader.SetFloat("material.shininess", ball.Material.Shininess);

            _lightShader.SetVector3("light.position", Light!.Position);
            _lightShader.SetVector4("light.ambient", Light.Ambient.ToVector4());
            _lightShader.SetVector4("light.diffuse", Light.Diffuse.ToVector4());
            _lightShader.SetVector4("light.specular", Light.Specular.ToVector4());


            ball.Draw();

            Tool.ThrowGLError();
        }
        public void DrawLine(Vector3 start, Vector3 end, float width, Transform transform, Color color)
        {
            using var line = new Line(start, end, width)
            {
                Transform = transform,
                Color = color
            };

            DrawLine(line);
        }
        public void DrawLine(Line line)
        {
            MakeCurrent();

            _lineShader.Use();

            GL.Enable(EnableCap.Blend);


            GL.BlendEquationSeparate(BlendEquationMode.FuncAdd, BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            _lineShader.SetVector4("color", line.Color.ToVector4());
            _lineShader.SetMatrix4("model", line.Transform.Matrix);
            _lineShader.SetMatrix4("view", Camera.GetViewMatrix());
            _lineShader.SetMatrix4("projection", Camera.GetProjectionMatrix());

            _lineShader.Use();

            line.Draw();

            Tool.ThrowGLError();
        }
        public void Dispose()
        {
            if (IsDisposed) return;

            _synchronization.Post(state =>
            {
                var g = (GraphicsContext)state!;

                GLFW.DestroyWindow(g._window);
                g._textureShader.Dispose();
                g._shader.Dispose();
                g._lightShader.Dispose();

            }, this);

            GC.SuppressFinalize(this);

            IsDisposed = true;
        }
        public unsafe void ReadImage(Image<BGRA32> image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            image.ThrowIfDisposed();
            MakeCurrent();

            //GL.ReadBuffer(ReadBufferMode.Front);
            GL.ReadBuffer(ReadBufferMode.ColorAttachment0);

            fixed (BGRA32* data = image.Data)
            {
                GL.ReadPixels(0, 0, image.Width, image.Height, PixelFormat.Bgra, PixelType.UnsignedByte, (IntPtr)data);
            }

            image.Flip(FlipMode.X);

            Tool.ThrowGLError();
        }
    }
}
