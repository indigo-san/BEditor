﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

using BEditor.Core.Data.ObjectData;
using BEditor.Core.Data.ProjectData;
using BEditor.Core.Data.PropertyData;
using BEditor.Core.Media;
using BEditor.Core.Properties;

namespace BEditor.Core.Data.EffectData
{
    [DataContract(Namespace = "")]
    public class AreaExpansion : ImageEffect
    {
        #region ImageEffect

        public override string Name => Resources.AreaExpansion;

        public override IEnumerable<PropertyElement> Properties => new PropertyElement[]
        {
            Top,
            Bottom,
            Left,
            Right,
            AdjustCoordinates
        };

        public override void Render(ref Image source, EffectRenderArgs args)
        {
            int top = (int)Top.GetValue(args.Frame);
            int bottom = (int)Bottom.GetValue(args.Frame);
            int left = (int)Left.GetValue(args.Frame);
            int right = (int)Right.GetValue(args.Frame);

            if (AdjustCoordinates.IsChecked && Parent.Effect[0] is ImageObject image)
            {
                image.Coordinate.CenterX.Optional = (right / 2) - (left / 2);
                image.Coordinate.CenterY.Optional = (top / 2) - (bottom / 2);
            }

            source.AreaExpansion(top, bottom, left, right);
        }

        #endregion

        public AreaExpansion()
        {
            Top = new(Clipping.TopMetadata);
            Bottom = new(Clipping.BottomMetadata);
            Left = new(Clipping.LeftMetadata);
            Right = new(Clipping.RightMetadata);
            AdjustCoordinates = new(Clipping.AdjustCoordinatesMetadata);
        }


        [DataMember(Order = 0)]
        [PropertyMetadata(nameof(Clipping.TopMetadata), typeof(Clipping))]
        public EaseProperty Top { get; private set; }

        [DataMember(Order = 1)]
        [PropertyMetadata(nameof(Clipping.BottomMetadata), typeof(Clipping))]
        public EaseProperty Bottom { get; private set; }

        [DataMember(Order = 2)]
        [PropertyMetadata(nameof(Clipping.LeftMetadata), typeof(Clipping))]
        public EaseProperty Left { get; private set; }

        [DataMember(Order = 3)]
        [PropertyMetadata(nameof(Clipping.RightMetadata), typeof(Clipping))]
        public EaseProperty Right { get; private set; }

        [DataMember(Order = 4)]
        [PropertyMetadata(nameof(Clipping.AdjustCoordinatesMetadata), typeof(Clipping))]
        public CheckProperty AdjustCoordinates { get; private set; }

    }
}
