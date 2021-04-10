﻿
using BEditor.Drawing.Pixel;

namespace BEditor.Drawing.PixelOperation
{
    public readonly unsafe struct ConvertToOperation<T1, T2> : IPixelOperation
        where T1 : unmanaged, IPixel<T1>, IPixelConvertable<T2>
        where T2 : unmanaged, IPixel<T2>
    {
        private readonly T1* _src;
        private readonly T2* _dst;

        public ConvertToOperation(T1* src, T2* dst)
        {
            _src = src;
            _dst = dst;
        }

        public readonly void Invoke(int p)
        {
            _src[p].ConvertTo(out _dst[p]);
        }
    }
}