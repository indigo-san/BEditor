﻿using System;

using BEditor.Resources;

namespace BEditor.Data
{
    internal interface IDirectProperty : IEditingProperty
    {
        public object Get(IEditingObject owner);
        public void Set(IEditingObject owner, object value);
    }

    internal interface IDirectProperty<TValue> : IDirectProperty, IEditingProperty<TValue>
    {
        object IDirectProperty.Get(IEditingObject owner)
        {
            return Get(owner)!;
        }

        void IDirectProperty.Set(IEditingObject owner, object value)
        {
            Set(owner, (TValue)value);
        }

        public new TValue Get(IEditingObject owner);

        public void Set(IEditingObject owner, TValue value);
    }

    /// <summary>
    /// A direct editing property.
    /// </summary>
    /// <typeparam name="TOwner">The type of the owner.</typeparam>
    /// <typeparam name="TValue">The type of the property.</typeparam>
    public class DirectEditingProperty<TOwner, TValue> : EditingProperty<TValue>, IDirectProperty<TValue>
        where TOwner : IEditingObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectEditingProperty{TOwner, TValue}"/> class.
        /// </summary>
        /// <param name="getter">Gets the current value of the property.</param>
        /// <param name="setter">Sets the value of the property.</param>
        /// <param name="key">The registry key.</param>
        public DirectEditingProperty(Func<TOwner, TValue> getter, Action<TOwner, TValue> setter, EditingPropertyRegistryKey key) : base(key)
        {
            (Getter, Setter) = (getter, setter);
        }

        /// <summary>
        /// Gets the getter function.
        /// </summary>
        public Func<TOwner, TValue> Getter { get; }

        /// <summary>
        /// Gets the setter function.
        /// </summary>
        public Action<TOwner, TValue> Setter { get; }

        TValue IDirectProperty<TValue>.Get(IEditingObject owner)
        {
            return Getter((TOwner)owner)!;
        }

        void IDirectProperty<TValue>.Set(IEditingObject owner, TValue value)
        {
            Setter((TOwner)owner, value);
        }

        /// <summary>
        /// Registers the direct property on another type.
        /// </summary>
        /// <typeparam name="TNewOwner">The type of the owner.</typeparam>
        /// <param name="getter">Gets the current value of the property.</param>
        /// <param name="setter">Sets the value of the property.</param>
        /// <param name="initializer">The <see cref="IEditingPropertyInitializer{T}"/> that initializes the local value of a property.</param>
        /// <param name="serializer">To serialize this property, specify the serializer.</param>
        public DirectEditingProperty<TNewOwner, TValue> WithOwner<TNewOwner>(
            Func<TNewOwner, TValue> getter,
            Action<TNewOwner, TValue> setter,
            IEditingPropertyInitializer<TValue>? initializer = null,
            IEditingPropertySerializer<TValue>? serializer = null)
            where TNewOwner : IEditingObject
        {
            var key = new EditingPropertyRegistryKey(Name, typeof(TNewOwner), Key.IsDisposable);

            if (EditingPropertyRegistry.IsRegistered(key))
            {
                throw new DataException(Strings.KeyHasAlreadyBeenRegisterd);
            }

            initializer ??= Initializer;
            serializer ??= Serializer;

            var property = new DirectEditingProperty<TNewOwner, TValue>(getter, setter, key)
            {
                Initializer = initializer,
                Serializer = serializer,
            };

            EditingPropertyRegistry.RegisterUnChecked(key, property);

            return property;
        }
    }
}