﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using BEditor.Data.Property;

using Microsoft.Extensions.DependencyInjection;

namespace BEditor.Data.Property
{
    /// <summary>
    /// Represents a property used by <see cref="EffectElement"/>.
    /// </summary>
    [DataContract]
    public class PropertyElement : EditorObject, IChild<EffectElement>, IPropertyElement, IHasId, IHasName
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

    /// <inheritdoc cref="PropertyElement"/>
    /// <typeparam name="T">Type of <see cref="PropertyMetadata"/></typeparam>
    [DataContract]
    public abstract class PropertyElement<T> : PropertyElement where T : PropertyElementMetadata
    {
        /// <inheritdoc cref="PropertyElement.PropertyMetadata"/>
        public new T? PropertyMetadata
        {
            get => base.PropertyMetadata as T;
            set => base.PropertyMetadata = value;
        }
    }

    /// <summary>
    /// The metadata of <see cref="BEditor.Data.Property.PropertyElement"/>.
    /// </summary>
    /// <param name="Name">The string displayed in the property header.</param>
    public record PropertyElementMetadata(string Name);
}
