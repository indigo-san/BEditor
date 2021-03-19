﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Reactive.Disposables;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using BEditor.Command;
using BEditor.Data.Bindings;
using BEditor.Data.Property;

namespace BEditor.Data.Property
{
    /// <summary>
    /// Represents a checkbox property.
    /// </summary>
    [DataContract]
    [DebuggerDisplay("IsChecked = {Value}")]
    public class CheckProperty : PropertyElement<CheckPropertyMetadata>, IEasingProperty, IBindable<bool>
    {
        #region Fields
        private bool _value;
        private List<IObserver<bool>>? _list;
        private IDisposable? _bindDispose;
        private IBindable<bool>? _bindable;
        private string? _bindHint;
        #endregion


        /// <summary>
        /// Initializes new instance of the <see cref="CheckProperty"/> class.
        /// </summary>
        /// <param name="metadata">Metadata of this property.</param>
        /// <exception cref="ArgumentNullException"><paramref name="metadata"/> is <see langword="null"/>.</exception>
        public CheckProperty(CheckPropertyMetadata metadata)
        {
            PropertyMetadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            _value = metadata.DefaultIsChecked;
        }

        private List<IObserver<bool>> Collection => _list ??= new();
        /// <inheritdoc/>
        [DataMember]
        public string? BindHint
        {
            get => _bindable?.GetString();
            private set => _bindHint = value;
        }
        /// <summary>
        /// Gets or sets the value of whether the item is checked or not.
        /// </summary>
        [DataMember]
        public bool Value
        {
            get => _value;
            set => SetValue(value, ref _value, DocumentProperty._valueArgs, this, state =>
            {
                foreach (var observer in state.Collection)
                {
                    try
                    {
                        observer.OnNext(state._value);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                    }
                }
            });
        }


        #region Methods

        /// <inheritdoc/>
        public void OnCompleted() { }

        /// <inheritdoc/>
        public void OnError(Exception error) { }

        /// <inheritdoc/>
        public void OnNext(bool value)
        {
            Value = value;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="observer"/> is <see langword="null"/>.</exception>
        public IDisposable Subscribe(IObserver<bool> observer)
        {
            return BindingHelper.Subscribe(Collection, observer, Value);
        }

        /// <inheritdoc/>
        public void Bind(IBindable<bool>? bindable)
        {
            Value = this.Bind(bindable, out _bindable, ref _bindDispose);
        }

        /// <inheritdoc/>
        protected override void OnLoad()
        {
            this.AutoLoad(ref _bindHint);
        }

        /// <summary>
        /// Create a command to change whether it is checked or not.
        /// </summary>
        /// <param name="value">New value for IsChecked</param>
        /// <returns>Created <see cref="IRecordCommand"/>.</returns>
        [Pure]
        public IRecordCommand ChangeIsChecked(bool value) => new ChangeCheckedCommand(this, value);

        #endregion


        private sealed class ChangeCheckedCommand : IRecordCommand
        {
            private readonly WeakReference<CheckProperty> _property;
            private readonly bool _value;

            public ChangeCheckedCommand(CheckProperty property, bool value)
            {
                _property = new(property);
                _value = value;
            }

            public string Name => CommandName.ChangeIsChecked;

            public void Do()
            {
                if (_property.TryGetTarget(out var target))
                {
                    target.Value = _value;
                }
            }
            public void Redo()
            {
                Do();
            }
            public void Undo()
            {
                if (_property.TryGetTarget(out var target))
                {
                    target.Value = !_value;
                }
            }
        }
    }

    /// <summary>
    /// The metadata of <see cref="CheckProperty"/>.
    /// </summary>
    /// <param name="Name">The string displayed in the property header.</param>
    /// <param name="DefaultIsChecked">The default value for <see cref="CheckProperty.Value"/>.</param>
    public record CheckPropertyMetadata(string Name, bool DefaultIsChecked = false) : PropertyElementMetadata(Name), IPropertyBuilder<CheckProperty>
    {
        /// <inheritdoc/>
        public CheckProperty Build()
        {
            return new(this);
        }
    }
}
