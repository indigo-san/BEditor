using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using BEditor.Drawing;
using BEditor.Drawing.Pixel;
using BEditor.Media.Decoding;

namespace BEditor.Extensions.MF.Decoding
{
    public sealed unsafe class VideoStream : IVideoStream
    {
        private readonly IntPtr _ptr;
        private long _current;

        public VideoStream(IntPtr ptr)
        {
            _ptr = ptr;
            var info = NativeMethods.VStream_GetInfo(ptr);

            Info = new(
                new string(info.Codec),
                Media.MediaType.Video,
                TimeSpan.FromSeconds(info.Duration),
                new(info.Width, info.Height),
                info.FrameNum,
                info.FrameRate);
        }

        public VideoStreamInfo Info { get; }

        public void Dispose()
        {
        }

        public Image<BGRA32> GetFrame(TimeSpan time)
        {
            if (!TryGetFrame(time, out var img)) throw new EndOfStreamException();
            return img;
        }

        public Image<BGRA32> GetNextFrame()
        {
            if (!TryGetNextFrame(out var img)) throw new EndOfStreamException();
            return img;
        }

        public bool TryGetFrame(TimeSpan time, [NotNullWhen(true)] out Image<BGRA32>? image)
        {
            return TryGetFrameCore((long)(time / Info.Duration * Info.NumberOfFrames), out image);
        }

        public bool TryGetNextFrame([NotNullWhen(true)] out Image<BGRA32>? image)
        {
            return TryGetFrameCore(_current + 1, out image);
        }

        private bool TryGetFrameCore(long position, [NotNullWhen(true)] out Image<BGRA32>? image)
        {
            _current = position;
            var size = Info.FrameSize;
            var byteSize = size.Width * size.Height * sizeof(Rgb32);
            var nativeImage = new NativeImage
            {
                Width = size.Width,
                Height = size.Height,
                Data = (Rgb32*)Marshal.AllocCoTaskMem(byteSize),
            };

            if (NativeMethods.VStream_TryGetFrame(_ptr, position, &nativeImage) is 1)
            {
                using var tmp = new Image<Rgb32>(size.Width, size.Height);
                fixed (Rgb32* dst = tmp.Data)
                {
                    Buffer.MemoryCopy(nativeImage.Data, dst, byteSize, byteSize);
                }

                image = tmp.Convert<Rgb32, BGRA32>();

                Marshal.FreeCoTaskMem((IntPtr)nativeImage.Data);
                return true;
            }
            else
            {
                image = null;
                Marshal.FreeCoTaskMem((IntPtr)nativeImage.Data);
                return false;
            }
        }
    }
}
