﻿// Double3.cs
//
// Copyright (C) BEditor
//
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.

using System.Runtime.InteropServices;

namespace BEditor.Compute
{
    /// <summary>
    /// A vector of 3 64-bit floating-point values.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Double3
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Double3"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public Double3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Double3"/> struct.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="z">The z.</param>
        public Double3(Double2 vector, double z)
        {
            X = vector.X;
            Y = vector.Y;
            Z = z;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Double3"/> struct.
        /// </summary>
        /// <param name="vector">The vector.</param>
        public Double3(Double3 vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Double3"/> struct.
        /// </summary>
        /// <param name="vector">The vector.</param>
        public Double3(Double4 vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets the z.
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// Gets or sets the xy.
        /// </summary>
        public Double2 XY
        {
            get => new(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        /// <summary>
        /// Gets or sets the xyz.
        /// </summary>
        public Double3 XYZ
        {
            get => this;
            set => this = value;
        }
    }
}