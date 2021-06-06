using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using BEditor.Drawing;
using BEditor.Drawing.Pixel;
using BEditor.Media;
using BEditor.Media.Decoding;

using SharpDX.MediaFoundation;

namespace BEditor.Extensions.MediaFoundation.Decoding
{
    public sealed unsafe class VideoStream : IVideoStream
    {
        private readonly SourceReader _reader;

        private readonly long _duration;

        // 現在のフレーム
        private long _current;

        public VideoStream(SourceReader reader)
        {
            _reader = reader;

            using var mediaType = reader.GetCurrentMediaType(SourceReaderIndex.FirstVideoStream);
            const double timebase = 10 * 1000 * 1000;
            _duration = reader.GetPresentationAttribute(SourceReaderIndex.MediaSource, PresentationDescriptionAttributeKeys.Duration);
            var duration = _duration / timebase;
            var frameSize = mediaType.Get(MediaTypeAttributeKeys.FrameSize);
            var size = new Size((int)(frameSize >> 32), (int)(frameSize & 0xffffffff));
            var framerate_ratio = mediaType.Get(MediaTypeAttributeKeys.FrameRate);
            var framerate = (int)(framerate_ratio >> 32) / (int)(framerate_ratio & 0xffffffff);

            Info = new(string.Empty, Media.MediaType.Video, TimeSpan.FromSeconds(duration), size, (int)(duration * framerate));
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
            if (position > Info.NumberOfFrames)
            {
                image = null;
                return false;
            }
            _current = position;
            var per = (float)position / Info.NumberOfFrames;
            _reader.SetCurrentPosition((long)(_duration * per));

            using var sample = _reader.ReadSample(SourceReaderIndex.FirstVideoStream, SourceReaderControlFlags.None, out _, out _, out _);
            using var buf = sample.ConvertToContiguousBuffer();
            var src = buf.Lock(out _, out _);
            using var img = new Image<RGB32>(Info.FrameSize.Width, Info.FrameSize.Height);
            fixed (void* dst = img.Data)
            {
                var bytes = img.DataSize;
                Buffer.MemoryCopy((void*)src, dst, bytes, bytes);
            }
            buf.Unlock();

            image = img.Convert<RGB32, BGRA32>();
            return true;
        }

        private struct RGB32 : IPixel<RGB32>, IPixelConvertable<BGRA32>
        {
            public byte R;
            public byte G;
            public byte B;

            public RGB32 Add(RGB32 foreground)
            {
                throw new NotImplementedException();
            }

            public RGB32 Blend(RGB32 foreground)
            {
                throw new NotImplementedException();
            }

            public void ConvertFrom(BGRA32 src)
            {
                R = src.R;
                G = src.G;
                B = src.B;
            }

            public void ConvertTo(out BGRA32 dst)
            {
                dst = new(R, G, B, 255);
            }

            public RGB32 Subtract(RGB32 foreground)
            {
                throw new NotImplementedException();
            }
        }
    }
}
