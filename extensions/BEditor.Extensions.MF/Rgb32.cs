using System;
using System.Runtime.InteropServices;

using BEditor.Drawing.Pixel;

namespace BEditor.Extensions.MF
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Rgb32 : IPixel<Rgb32>, IPixelConvertable<BGRA32>
    {
        public byte R;
        public byte G;
        public byte B;
        public byte Reserve;

        public Rgb32 Add(Rgb32 foreground)
        {
            throw new NotImplementedException();
        }

        public Rgb32 Blend(Rgb32 foreground)
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

        public Rgb32 Subtract(Rgb32 foreground)
        {
            throw new NotImplementedException();
        }
    }
}
