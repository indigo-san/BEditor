﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace BEditor.Media
{
    [Serializable]
    public readonly struct Frame : IEquatable<Frame>, ISerializable
    {
        public static readonly Frame MaxValue = new(int.MaxValue);
        public static readonly Frame MinValue = new(int.MinValue);
        public static readonly Frame Zero = new();

        public Frame(int value)
        {
            Value = value;
        }
        public Frame(SerializationInfo info, StreamingContext context)
        {
            Value = info.GetInt32(nameof(Value));
        }

        public int Value { get; }

        public double ToSeconds(double framerate) => Value / framerate;
        public double ToMinutes(double framerate) => ToSeconds(framerate) * 60;
        public double ToHours(double framerate) => ToMinutes(framerate) * 60;
        public static Frame FromSeconds(double seconds, double framerate) => new((int)(seconds * framerate));
        public static Frame FromMinutes(double minutes, double framerate) => FromSeconds(minutes * 60, framerate);
        public static Frame FromHours(double hours, double framerate) => FromMinutes(hours * 60, framerate);
        public override readonly bool Equals(object? obj)
            => obj is Frame frame && Equals(frame);
        public readonly bool Equals(Frame other)
            => Value == other.Value;
        public override readonly int GetHashCode()
            => HashCode.Combine(Value);
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Value), Value);
        }

        public static bool operator ==(Frame left, Frame right) => left.Equals(right);
        public static bool operator !=(Frame left, Frame right) => !(left == right);
        public static bool operator <(Frame left, Frame right) => left.Value < right.Value;
        public static bool operator >(Frame left, Frame right) => left.Value > right.Value;
        public static bool operator <=(Frame left, Frame right) => left.Value <= right.Value;
        public static bool operator >=(Frame left, Frame right) => left.Value >= right.Value;
        public static Frame operator +(Frame left, Frame right) => new(left.Value + right.Value);
        public static Frame operator -(Frame left, Frame right) => new(left.Value - right.Value);
        public static Frame operator /(Frame left, Frame right) => new(left.Value / right.Value);
        public static Frame operator *(Frame left, Frame right) => new(left.Value * right.Value);

        public static implicit operator int(Frame frame) => frame.Value;
        public static implicit operator Frame(int value) => new(value);
    }
}
