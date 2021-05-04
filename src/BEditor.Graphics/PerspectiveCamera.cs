﻿using System.Numerics;

using MathHelper = OpenTK.Mathematics.MathHelper;

namespace BEditor.Graphics
{
    /// <summary>
    /// Represents the perspective camera.
    /// </summary>
    public class PerspectiveCamera : Camera
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PerspectiveCamera"/> class.
        /// </summary>
        /// <param name="position">The position of the camera.</param>
        /// <param name="aspectRatio">The aspect ratio of the camera's viewport.</param>
        public PerspectiveCamera(Vector3 position, float aspectRatio) : base(position)
        {
            AspectRatio = aspectRatio;
        }

        /// <summary>
        /// Gets the aspect ratio of the camera's viewport.
        /// </summary>
        public float AspectRatio { get; set; }

        /// <inheritdoc/>
        public override Matrix4x4 GetProjectionMatrix()
        {
            return Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Fov), AspectRatio, Near, Far);
        }
    }
}