﻿using System;
using System.Collections.Generic;

using BEditor.Drawing;
using BEditor.Drawing.Pixel;

namespace BEditor.Extensions.AviUtl.Effects
{
    public sealed class RadiantBlur : IMappedEffect
    {
        public string Name => "放射ブラー";

        public void Apply(ref Image<BGRA32> image, ObjectTable table, Dictionary<string, object> @params)
        {
            throw new NotImplementedException();
        }
    }
}
