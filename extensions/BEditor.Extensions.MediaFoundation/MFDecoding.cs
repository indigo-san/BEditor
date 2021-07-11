using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BEditor.Extensions.MediaFoundation.Decoding;
using BEditor.Media;
using BEditor.Media.Decoding;

namespace BEditor.Extensions.MediaFoundation
{
    public sealed class MFDecoding : IRegisterdDecoding
    {
        public string Name => "MediaFoundation";

        public IInputContainer? Open(string file, MediaOptions options)
        {
            return new InputContainer(file, options);
        }

        public IEnumerable<string> SupportExtensions()
        {
            yield return ".mp4";
            yield return ".m4a";
            yield return ".m4v";
            yield return ".mov";
            yield return ".avi";
            yield return ".3g2";
            yield return ".3gp";
            yield return ".3gpp";
            yield return ".asf";
            yield return ".wma";
            yield return ".wmv";
            yield return ".mp3";
            yield return ".wav";
            yield return ".aac";
            yield return ".adts";
        }
    }
}
