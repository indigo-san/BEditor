using System.Runtime.InteropServices;

namespace BEditor.Extensions.MF
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct NativeVideoStreamInfo
    {
        public char* Codec;
        public double Duration;
        public int Width;
        public int Height;
        public int FrameNum;
        public int FrameRate;
    }
}
