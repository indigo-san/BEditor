// ObjectMetadata.cs
//
// Copyright (C) BEditor
//
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.

using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

using BEditor.Drawing;

namespace BEditor.Data
{
    /// <summary>
    /// The metadata of <see cref="ObjectElement"/>.
    /// </summary>
    public class ObjectMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectMetadata"/> class.
        /// </summary>
        /// <param name="name">The name of the effect element.</param>
        /// <param name="createFunc">Create a new instance of the <see cref="ObjectMetadata"/> object.</param>
        /// <param name="type">The type of the object that inherits from <see cref="ObjectMetadata"/>.</param>
        public ObjectMetadata(string name, Func<ObjectElement> createFunc, Type type)
        {
            Name = name;
            CreateFunc = createFunc;
            Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectMetadata"/> class.
        /// </summary>
        /// <param name="name">The name of the object element.</param>
        /// <param name="create">This <see cref="Func{TResult}"/> gets a new instance of the <see cref="ObjectElement"/> object.</param>
        public ObjectMetadata(string name, Expression<Func<ObjectElement>> create)
            : this(name, create.Compile(), ((NewExpression)create.Body).Type)
        {
        }

        /// <summary>
        /// Gets the loaded <see cref="ObjectMetadata"/>.
        /// </summary>
        public static ObservableCollection<ObjectMetadata> LoadedObjects { get; } = new();

        /// <summary>
        /// Gets or sets the name of the object element.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Create a new instance of the <see cref="ObjectElement"/> object.
        /// </summary>
        public Func<ObjectElement> CreateFunc { get; }

        /// <summary>
        /// Gets or sets the type of the object that inherits from <see cref="ObjectElement"/>.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the accent color.
        /// </summary>
        public Color AccentColor { get; private set; } = Color.FromUInt32(0xff304fee);

        /// <summary>
        /// Gets the path data of an icon.
        /// </summary>
        public string PathIcon { get; private set; } = string.Empty;

        /// <summary>
        /// Creates an instance from a file name.
        /// </summary>
        public Func<string, ObjectElement>? CreateFromFile { get; private set; }

        /// <summary>
        /// Check to see if the file name is supported.
        /// </summary>
        public Func<string, bool>? IsSupported { get; private set; }

        /// <summary>
        /// Create the <see cref="ObjectMetadata"/>.
        /// </summary>
        /// <typeparam name="T">The type of object that inherits from <see cref="ObjectElement"/>.</typeparam>
        /// <param name="name">The name of the object element.</param>
        /// <returns>A new instance of <see cref="ObjectMetadata"/>.</returns>
        [Obsolete("Use Create{T}(string, Color?, string, Func{string, T}).")]
        public static ObjectMetadata Create<T>(string name)
            where T : ObjectElement, new()
        {
            return new(name, () => new T(), typeof(T));
        }

        /// <summary>
        /// Create the <see cref="ObjectMetadata"/>.
        /// </summary>
        /// <typeparam name="T">The type of object that inherits from <see cref="ObjectElement"/>.</typeparam>
        /// <param name="name">The name of the object element.</param>
        /// <param name="accentColor">The accent color.</param>
        /// <param name="pathIcon">The path data of an icon.</param>
        /// <param name="createFromFile">Creates an instance from a file name.</param>
        /// <param name="isSupported">Check to see if the file name is supported.</param>
        /// <returns>A new instance of <see cref="ObjectMetadata"/>.</returns>
        public static ObjectMetadata Create<T>(string name, Color? accentColor = null, string pathIcon = "", Func<string, T>? createFromFile = null, Func<string, bool>? isSupported = null)
            where T : ObjectElement, new()
        {
            return new(name, () => new T(), typeof(T))
            {
                AccentColor = accentColor ?? Color.FromUInt32(0xff304fee),
                PathIcon = pathIcon,
                CreateFromFile = createFromFile,
                IsSupported = isSupported,
            };
        }
    }
}