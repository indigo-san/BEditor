using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BEditor.Media.Decoding;

namespace BEditor.Extensions.MF.Decoding
{
    public sealed unsafe class InputContainer : IInputContainer
    {
        private readonly IntPtr _pointer;

        public InputContainer(string file)
        {
            _pointer = NativeMethods.NewInputContainer(file);

            if(_pointer == IntPtr.Zero)
            {
                var msg = NativeMethods.GetError();
                throw new Exception(msg);
            }

            var video = NativeMethods.GetVideoStream(_pointer);
            Video = new IVideoStream[]
            {
                new VideoStream(video),
            };
            Audio = Array.Empty<IAudioStream>();
        }

        public IVideoStream[] Video { get; }

        public IAudioStream[] Audio { get; }

        public MediaInfo Info { get; }

        public void Dispose()
        {
            NativeMethods.DeleteInputContainer(_pointer);
        }
    }
}
