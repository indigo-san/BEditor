﻿using System;

using BEditor.Drawing;
using BEditor.Drawing.Pixel;
using BEditor.Drawing.PixelOperation;

using NUnit.Framework;

namespace OpenCLTest
{
    public class SepiaTest
#if !GITHUB_ACTIONS
        : IDisposable
#endif
    {
        [Test]
        public void Cpu()
        {
            using var img = Image<BGRA32>.FromFile(BinarizationTest.FilePath);

            img.Sepia();
        }

#if !GITHUB_ACTIONS
        private readonly DrawingContext context;

        public SepiaTest()
        {
            context = DrawingContext.Create(0);

            var op = (SepiaOperation)default;
            var prog = context.Context.CreateProgram(op.GetSource());
            var key = op.GetType().Name;

            context.Programs.Add(key, prog);
        }

        [Test]
        public void Gpu()
        {
            using var img = Image<BGRA32>.FromFile(BinarizationTest.FilePath);

            img.Sepia(context);
        }

        public void Dispose()
        {
            context.Dispose();

            GC.SuppressFinalize(this);
        }
#endif
    }
}