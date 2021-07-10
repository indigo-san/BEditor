// EasingMetadata.cs
//
// Copyright (C) BEditor
//
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BEditor.Data.Property.Easing
{
    /// <summary>
    /// The metadata of <see cref="EasingFunc"/>.
    /// </summary>
    public class EasingMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EasingMetadata"/> class.
        /// </summary>
        /// <param name="name">The name of the effect element.</param>
        /// <param name="createFunc">Create a new instance of the <see cref="EasingMetadata"/> object.</param>
        /// <param name="type">The type of the object that inherits from <see cref="EasingMetadata"/>.</param>
        public EasingMetadata(string name, Func<EasingFunc> createFunc, Type type)
        {
            Name = name;
            CreateFunc = createFunc;
            Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EasingMetadata"/> class.
        /// </summary>
        /// <param name="name">The name of the easing function.</param>
        /// <param name="create">This <see cref="Func{TResult}"/> gets a new instance of the <see cref="EasingFunc"/> object.</param>
        public EasingMetadata(string name, Expression<Func<EasingFunc>> create)
            : this(name, create.Compile(), ((NewExpression)create.Body).Type)
        {
        }

        /// <summary>
        /// Gets the loaded <see cref="EasingMetadata"/>.
        /// </summary>
        public static List<EasingMetadata> LoadedEasingFunc { get; } = new()
        {
            Create<PrimitiveEasing>("Primitive"),
        };

        /// <summary>
        /// Gets or sets the name of the easing function.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Create a new instance of the <see cref="EasingFunc"/> object.
        /// </summary>
        public Func<EasingFunc> CreateFunc { get; }

        /// <summary>
        /// Gets or sets the type of the object that inherits from <see cref="EasingFunc"/>.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Create the <see cref="EasingMetadata"/>.
        /// </summary>
        /// <typeparam name="T">The type of object that inherits from EasingFunc.</typeparam>
        /// <param name="name">The name of the easing function.</param>
        /// <returns>A new instance of <see cref="EasingMetadata"/>.</returns>
        public static EasingMetadata Create<T>(string name)
                where T : EasingFunc, new()
        {
            return new(name, () => new T(), typeof(T));
        }
    }
}