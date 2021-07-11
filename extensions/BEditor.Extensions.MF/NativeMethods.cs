using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BEditor.Extensions.MF
{
    internal unsafe class NativeMethods
    {
        const string LibraryName = "BEditor.Extensions.MF.Extern.dll";

        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        public static extern string GetError();

        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        public static extern int Initialize();

        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        public static extern int Uninitialize();

        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        public static extern IntPtr NewInputContainer(string file);

        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        public static extern void DeleteInputContainer(IntPtr input);

        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        public static extern IntPtr GetVideoStream(IntPtr input);

        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        public static extern IntPtr GetAudioStream(IntPtr input);

        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        public static extern int VStream_TryGetFrame(IntPtr stream, long popsition, NativeImage* image);

        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        public static extern NativeVideoStreamInfo VStream_GetInfo(IntPtr stream);
    }
}
