﻿using System;
using System.ComponentModel;
using System.Text.Json;

namespace BEditor.Data.Property
{
    /// <summary>
    /// Represents a property used by <see cref="EffectElement"/>.
    /// </summary>
    public class PropertyElement : EditingObject, IChild<EffectElement>, IPropertyElement, IHasName
    {
        private static readonly PropertyChangedEventArgs _metadataArgs = new(nameof(PropertyMetadata));
        private PropertyElementMetadata? _propertyMetadata;
        private int? id;
        private WeakReference<EffectElement?>? parent;

        /// <inheritdoc/>
        public virtual EffectElement Parent
        {
            get
            {
                parent ??= new(null!);

                if (parent.TryGetTarget(out var p))
                {
                    return p;
                }

                return null!;
            }
            set => (parent ??= new(null!)).SetTarget(value);
        }

        /// <summary>
        /// Gets or sets the metadata for this <see cref="PropertyElement"/>.
        /// </summary>
        public PropertyElementMetadata? PropertyMetadata
        {
            get => _propertyMetadata;
            set => SetValue(value, ref _propertyMetadata, _metadataArgs);
        }

        /// <inheritdoc/>
        public int Id => (id ??= Parent?.Children?.IndexOf(this)) ?? -1;

        /// <inheritdoc/>
        public string Name => _propertyMetadata?.Name ?? Id.ToString();
    }
}
