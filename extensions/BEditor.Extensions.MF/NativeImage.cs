using System.Runtime.InteropServices;

namespace BEditor.Extensions.MF
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct NativeImage
    {
        public int Width;
        public int Height;
        public Rgb32* Data;
    }
}
