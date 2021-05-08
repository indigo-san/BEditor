﻿using BEditor.Media;

namespace BEditor.Data
{
    /// <summary>
    /// Represents data that is passed to <see cref="EffectElement"/> when it is applied.
    /// </summary>
    public class EffectApplyArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EffectApplyArgs"/> class.
        /// </summary>
        public EffectApplyArgs(Frame frame, RenderType type = RenderType.Preview)
        {
            Frame = frame;
            Type = type;
        }

        /// <summary>
        /// Gets the frame to render.
        /// </summary>
        public Frame Frame { get; }

        /// <summary>
        /// Gets or sets a value that indicates the current state of the process.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// Gets the rendering type.
        /// </summary>
        public RenderType Type { get; }
    }
}