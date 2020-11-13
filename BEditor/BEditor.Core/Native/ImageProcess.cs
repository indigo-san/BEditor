﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Text;

namespace BEditor.Core.Native
{
    public unsafe static class ImageProcess
    {
        private const string dll = "BEditor.Extern";

        [Pure, DllImport(dll, EntryPoint = "ImageCreate1")]
        public static extern string Create(out IntPtr returnmat);

        [Pure, DllImport(dll, EntryPoint = "ImageCreate2")]
        public static extern string Create(int width, int height, int type, out IntPtr returnmat);

        [Pure, DllImport(dll, EntryPoint = "ImageCreate3")]
        public static extern string Create(int width, int height, IntPtr data, int type, out IntPtr returnmat);

        [Pure, DllImport(dll, EntryPoint = "ImageCreate4")]
        public static extern string Create(IntPtr mat, int x, int y, int width, int height, out IntPtr returnmat);


        [Pure, DllImport(dll, EntryPoint = "ImageRead")]
        public static extern string Decode(string filename, out IntPtr returnmat);

        [Pure, DllImport(dll, EntryPoint = "ImageDecode")]
        public static extern string Decode(byte[] buf, IntPtr bufLength, int flags, out IntPtr returnmat);

        [Pure, DllImport(dll, EntryPoint = "ImageDecode")]
        public static extern unsafe string Decode(byte* buf, IntPtr bufLength, int flags, out IntPtr returnmat);

        [Pure, DllImport(dll, EntryPoint = "ImageRelease")]
        public static extern string Release(IntPtr mat);

        [Pure, DllImport(dll, EntryPoint = "ImageDelete")]
        public static extern string Delete(IntPtr mat);


        [Pure, DllImport(dll, EntryPoint = "ImageData")]
        public static extern string Data(IntPtr mat, out byte* data);

        [Pure, DllImport(dll, EntryPoint = "ImageDataEnd")]
        public static extern string DataEnd(IntPtr mat, out IntPtr dataend);

        [Pure, DllImport(dll, EntryPoint = "ImageWidth")]
        public static extern string Width(IntPtr mat, out int width);

        [Pure, DllImport(dll, EntryPoint = "ImageHeight")]
        public static extern string Height(IntPtr mat, out int height);

        [Pure, DllImport(dll, EntryPoint = "ImageStep")]
        public static extern string Step(IntPtr mat, out IntPtr step);

        [Pure, DllImport(dll, EntryPoint = "ImageDimension")]
        public static extern string Dimension(IntPtr mat, out int dim);


        [Pure, DllImport(dll, EntryPoint = "ImageType")]
        public static extern string Type(IntPtr mat, out int type);

        [Pure, DllImport(dll, EntryPoint = "ImageElemSize")]
        public static extern string ElemSize(IntPtr mat, out IntPtr elemsize);

        [Pure, DllImport(dll, EntryPoint = "ImageIsContinuous")]
        public static extern string IsContinuous(IntPtr mat, out int value);

        [Pure, DllImport(dll, EntryPoint = "ImageIsSubmatrix")]
        public static extern string IsSubmatrix(IntPtr mat, out int value);

        [Pure, DllImport(dll, EntryPoint = "ImageDepth")]
        public static extern string Depth(IntPtr mat, out int depth);

        [Pure, DllImport(dll, EntryPoint = "ImageChannels")]
        public static extern string Channels(IntPtr mat, out int ch);

        [Pure, DllImport(dll, EntryPoint = "ImageTotal")]
        public static extern string Total(IntPtr mat, out IntPtr total);

        [Pure, DllImport(dll, EntryPoint = "ImageClone")]
        public static extern string Clone(IntPtr mat, out IntPtr returnmat);

        [Pure, DllImport(dll, EntryPoint = "ImageSave1")]
        public static extern string Save(IntPtr mat, string filename, int[] @params, int paramLength, out int returnValue);

        [Pure, DllImport(dll, EntryPoint = "ImageSave2")]
        public static extern string Save(IntPtr mat, string filename, out int returnValue);

        [Pure, DllImport(dll, EntryPoint = "ImageSubMatrix")]
        public static extern string SubMatrix(IntPtr mat, int rowSt, int rowEd, int colSt, int colEd, out IntPtr returnmat);

        [Pure, DllImport(dll, EntryPoint = "ImageCopyTo")]
        public static extern string CopyTo(IntPtr mat, out IntPtr outmat);


        [Pure, DllImport(dll, EntryPoint = "ImageFlip")]
        public static extern string Flip(IntPtr mat, int mode);

        [Pure, DllImport(dll, EntryPoint = "ImageAreaExpansion1")]
        public static extern string AreaExpansion(IntPtr mat, int top, int bottom, int left, int right);

        [Pure, DllImport(dll, EntryPoint = "ImageAreaExpansion2")]
        public static extern string AreaExpansion(IntPtr mat, int width, int height);

        [Pure, DllImport(dll, EntryPoint = "ImageBlur")]
        public static extern string Blur(IntPtr mat, int blursize, bool alphablur);

        [Pure, DllImport(dll, EntryPoint = "ImageGaussianBlur")]
        public static extern string GaussianBlur(IntPtr mat, int blursize, bool alphablur);

        [Pure, DllImport(dll, EntryPoint = "ImageMedianBlur")]
        public static extern string MedianBlur(IntPtr mat, int blursize, bool alphablur);

        [Pure, DllImport(dll, EntryPoint = "ImageAdd")]
        public static extern string Add(IntPtr @base, IntPtr src1);

        [Pure, DllImport(dll, EntryPoint = "ImageDilate")]
        public static extern string Dilate(IntPtr mat, int f);

        [Pure, DllImport(dll, EntryPoint = "ImageErode")]
        public static extern string Erode(IntPtr mat, int f);

        [Pure, DllImport(dll, EntryPoint = "ImageClip")]
        public static extern string Clip(IntPtr mat, int top, int bottom, int left, int right, out IntPtr returnmat);


        [Pure, DllImport(dll, EntryPoint = "ImageEllipse")]
        public static extern string Ellipse(int width, int height, int line, float r, float g, float b, out IntPtr mat);
    }
}
