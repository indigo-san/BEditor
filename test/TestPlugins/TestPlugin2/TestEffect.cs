﻿using System.Collections.Generic;

using BEditor.Command;
using BEditor.Data;
using BEditor.Data.Primitive;
using BEditor.Data.Property;
using BEditor.Drawing;
using BEditor.Drawing.Pixel;

namespace TestPlugin2
{
    public class TestEffect : ImageEffect
    {
        public static readonly CheckPropertyMetadata CheckMetadata = new("チェックボックス");

        public TestEffect()
        {
            Check = new(CheckMetadata);
        }

        public override string Name => nameof(TestEffect);
        public override IEnumerable<PropertyElement> Properties => new PropertyElement[]
        {
            Check
        };
        [DataMember]
        public CheckProperty Check { get; private set; }

        public override void Render(EffectRenderArgs<Image<BGRA32>> args)
        {
            if (Check.Value)
                args.Value.Blur(50);
        }
        protected override void OnLoad()
        {
            Check.Load(CheckMetadata);
        }
        protected override void OnUnload()
        {
            Check.Unload();
        }
    }
}