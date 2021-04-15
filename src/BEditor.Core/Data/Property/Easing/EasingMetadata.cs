﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BEditor.Data.Property.Easing
{
    /// <summary>
    /// The metadata of <see cref="EasingFunc"/>.
    /// </summary>
    /// <param name="Name">The name of the easing function.</param>
    /// <param name="CreateFunc">This <see cref="Func{TResult}"/> gets a new instance of the <see cref="EasingFunc"/> object.</param>
    /// <param name="Type">The type of the object that inherits from <see cref="EasingFunc"/>.</param>
    public record EasingMetadata(string Name, Func<EasingFunc> CreateFunc, Type Type)
    {
        /// <summary>
        /// The metadata of <see cref="EasingFunc"/>.
        /// </summary>
        /// <param name="Name">The name of the easing function.</param>
        /// <param name="Create">This <see cref="Func{TResult}"/> gets a new instance of the <see cref="EasingFunc"/> object.</param>
        public EasingMetadata(string Name, Expression<Func<EasingFunc>> Create) : this(Name, Create.Compile(), ((NewExpression)Create.Body).Type)
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
        /// Create the <see cref="EasingMetadata"/>.
        /// </summary>
        /// <typeparam name="T">The type of object that inherits from EasingFunc.</typeparam>
        /// <param name="Name">The name of the easing function.</param>
        /// <returns>A new instance of <see cref="EasingMetadata"/>.</returns>
        public static EasingMetadata Create<T>(string Name) where T : EasingFunc, new()
        {
            return new(Name, () => new T(), typeof(T));
        }
    }
}