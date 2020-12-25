﻿using System;
using System.Collections.Generic;
using System.Text;

using BEditor.Drawing;
using BEditor.Drawing.Pixel;

namespace BEditor.Media.Decoder
{
    public interface IVideoDecoder : IDisposable
    {
        public int Fps { get; }
        public Frame FrameCount { get; }
        public int Width { get; }
        public int Height { get; }

        public Image<BGRA32> Read(Frame frame);
    }
}