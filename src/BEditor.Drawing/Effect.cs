﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BEditor.Drawing.Pixel;
using BEditor.Drawing.Process;

using SkiaSharp;

namespace BEditor.Drawing
{
    public static unsafe partial class Image
    {
        internal static double Set255(double value)
        {
            if (value > 255) return 255;
            else if (value < 0) return 0;

            return value;
        }

        internal static float Set255(float value)
        {
            if (value > 255) return 255;
            else if (value < 0) return 0;

            return value;
        }

        internal static double Set255Round(double value)
        {
            if (value > 255) return 255;
            else if (value < 0) return 0;

            return Math.Round(value);
        }

        internal static float Set255Round(float value)
        {
            if (value > 255) return 255;
            else if (value < 0) return 0;

            return MathF.Round(value);
        }

        public static void PixelProcess<IProcess>(int size, in IProcess process) where IProcess : IPixelProcess
        {
            Parallel.For(0, size, process.Invoke);
        }

        public static void PixelProcess<IProcess1, IProcess2>(int size, IProcess1 process1, IProcess2 process2)
            where IProcess1 : IPixelProcess
            where IProcess2 : IPixelProcess
        {
            Parallel.For(0, size, i =>
            {
                process1.Invoke(i);
                process2.Invoke(i);
            });
        }

        public static void PixelProcess<IProcess1, IProcess2, IProcess3>(int size, IProcess1 process1, IProcess2 process2, IProcess3 process3)
            where IProcess1 : IPixelProcess
            where IProcess2 : IPixelProcess
            where IProcess3 : IPixelProcess
        {
            Parallel.For(0, size, i =>
            {
                process1.Invoke(i);
                process2.Invoke(i);
                process3.Invoke(i);
            });
        }

        public static void PixelProcess<IProcess1, IProcess2, IProcess3, IProcess4>(int size, IProcess1 process1, IProcess2 process2, IProcess3 process3, IProcess4 process4)
            where IProcess1 : IPixelProcess
            where IProcess2 : IPixelProcess
            where IProcess3 : IPixelProcess
            where IProcess4 : IPixelProcess
        {
            Parallel.For(0, size, i =>
            {
                process1.Invoke(i);
                process2.Invoke(i);
                process3.Invoke(i);
                process4.Invoke(i);
            });
        }

        public static void Grayscale(this Image<BGRA32> image)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));
            image.ThrowIfDisposed();

            fixed (BGRA32* data = image.Data)
            {
                PixelProcess(image.Data.Length, new GrayscaleProcess(data, data));
            }
        }

        public static void Sepia(this Image<BGRA32> image)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));
            image.ThrowIfDisposed();

            fixed (BGRA32* data = image.Data)
            {
                PixelProcess(image.Data.Length, new SepiaProcess(data, data));
            }
        }

        public static void Negaposi(this Image<BGRA32> image, byte red, byte green, byte blue)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));
            image.ThrowIfDisposed();

            fixed (BGRA32* data = image.Data)
            {
                PixelProcess(image.Data.Length, new NegaposiProcess(data, data, red, green, blue));
            }
        }

        public static void Xor(this Image<BGRA32> image)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));
            image.ThrowIfDisposed();

            fixed (BGRA32* data = image.Data)
            {
                PixelProcess(image.Data.Length, new XorProcess(data, data));
            }
        }

        public static void Brightness(this Image<BGRA32> image, short brightness)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));
            image.ThrowIfDisposed();
            brightness = Math.Clamp(brightness, (short)-255, (short)255);

            fixed (BGRA32* data = image.Data)
            {
                PixelProcess(image.Data.Length, new BrightnessProcess(data, data, brightness));
            }
        }

        public static void Contrast(this Image<BGRA32> image, short contrast)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));
            image.ThrowIfDisposed();
            contrast = Math.Clamp(contrast, (short)-255, (short)255);

            using var lut = new UnmanagedArray<byte>(256);
            for (var i = 0; i < 256; i++)
            {
                lut[i] = (byte)Set255Round(((1d + (contrast / 255d)) * (i - 128d)) + 128d);
            }

            fixed (BGRA32* data = image.Data)
            {
                PixelProcess(image.Data.Length, new ContrastProcess(data, data, (byte*)lut.Pointer));
            }
        }

        public static void Gamma(this Image<BGRA32> image, float gamma)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));
            image.ThrowIfDisposed();
            gamma = Math.Clamp(gamma, 0.01f, 3f);

            using var lut = new UnmanagedArray<byte>(256);
            for (var i = 0; i < 256; i++)
            {
                lut[i] = (byte)Set255Round(Math.Pow(i / 255.0, 1.0 / gamma) * 255);
            }

            fixed (BGRA32* data = image.Data)
            {
                PixelProcess(image.Data.Length, new GammaProcess(data, data, (byte*)lut.Pointer));
            }
        }

        public static void RGBColor(this Image<BGRA32> image, short red, short green, short blue)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));
            image.ThrowIfDisposed();
            red = Math.Clamp(red, (short)-255, (short)255);
            green = Math.Clamp(green, (short)-255, (short)255);
            blue = Math.Clamp(blue, (short)-255, (short)255);

            fixed (BGRA32* data = image.Data)
            {
                PixelProcess(image.Data.Length, new RGBColorProcess(data, data, red, green, blue));
            }
        }

        public static void Normalize(this Image<BGRA32> image)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));
            image.ThrowIfDisposed();

            fixed (BGRA32* raw = image.Data)
            {
                byte r_min = 255;
                byte g_min = 255;
                byte b_min = 255;
                byte r_max = 0;
                byte g_max = 0;
                byte b_max = 0;

                var data = (IntPtr)raw;
                for (var i = 0; i < image.Data.Length; i++)
                {
                    // 最小値
                    if (raw[i].R < r_min) r_min = raw[i].R;
                    if (raw[i].G < g_min) g_min = raw[i].G;
                    if (raw[i].B < b_min) b_min = raw[i].B;

                    // 最大値
                    if (raw[i].R > r_max) r_max = raw[i].R;
                    if (raw[i].G > g_max) g_max = raw[i].G;
                    if (raw[i].B > b_max) b_max = raw[i].B;
                }

                // 比率を求める
                var r_ratio = 255 / (r_max - r_min);
                var g_ratio = 255 / (g_max - g_min);
                var b_ratio = 255 / (b_max - b_min);

                Parallel.For(0, image.Data.Length, i =>
                {
                    var raw = (BGRA32*)data;

                    raw[i].R = (byte)Set255Round(r_ratio * Math.Max(raw[i].R - r_min, 0));
                    raw[i].G = (byte)Set255Round(g_ratio * Math.Max(raw[i].G - g_min, 0));
                    raw[i].B = (byte)Set255Round(b_ratio * Math.Max(raw[i].B - b_min, 0));
                });
            }
        }

        public static void Binarization(this Image<BGRA32> image, byte value)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));
            image.ThrowIfDisposed();

            fixed (BGRA32* data = image.Data)
            {
                PixelProcess(image.Data.Length, new BinarizationProcess(data, data, value));
            }
        }

        public static void Noise(this Image<BGRA32> image, byte value)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));
            image.ThrowIfDisposed();

            fixed (BGRA32* data = image.Data)
            {
                // Randomを
                var rand = new Random();
                PixelProcess(image.Data.Length, new NoiseProcess(data, data, value, rand));
            }
        }

        // ここのアロケーションどうにかする
        public static void Diffusion(this Image<BGRA32> image, byte value)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));
            image.ThrowIfDisposed();
            value = Math.Clamp(value, (byte)0, (byte)30);

            fixed (BGRA32* data = image.Data)
            {
                var raw = (IntPtr)data;
                var rand = new Random();

                Parallel.For(0, image.Height, y =>
                {
                    Parallel.For(0, image.Width, x =>
                    {
                        var data = (BGRA32*)raw;
                        // 取得する座標
                        var dy = Math.Abs(y + rand.Next(-value, value));
                        var dx = Math.Abs(x + rand.Next(-value, value));
                        int sPos;
                        var dPos = (y * image.Width) + x;

                        // 範囲外はデフォルトの値を使用する
                        if ((dy > image.Height) || (dx > image.Width))
                        {
                            sPos = dPos;
                        }
                        else
                        {
                            var sRow = dy * image.Width;
                            sPos = sRow + dx;
                        }

                        data[dPos].R = data[sPos].R;
                        data[dPos].G = data[sPos].G;
                        data[dPos].B = data[sPos].B;
                    });
                });
            }
        }
    }
}