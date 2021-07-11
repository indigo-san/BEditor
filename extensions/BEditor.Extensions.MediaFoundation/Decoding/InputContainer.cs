using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BEditor.Drawing;
using BEditor.Media.Decoding;

using SharpDX.MediaFoundation;

namespace BEditor.Extensions.MediaFoundation.Decoding
{
    public sealed class InputContainer : IInputContainer
    {
        public InputContainer(string file, MediaOptions options)
        {
            using var attr = new MediaAttributes(1);
            using var videoMediaType = new MediaType();
            using var audioMediaType = new MediaType();

            //SourceReaderに動画のパスを設定
            attr.Set(SourceReaderAttributeKeys.EnableVideoProcessing.Guid, true);
            using var reader = new SourceReader(file, attr);

            //出力メディアタイプをRGB32bitに設定
            videoMediaType.Set(MediaTypeAttributeKeys.MajorType, MediaTypeGuids.Video);
            videoMediaType.Set(MediaTypeAttributeKeys.Subtype, VideoFormatGuids.Rgb32);
            //
            reader.SetCurrentMediaType(SourceReaderIndex.FirstVideoStream, videoMediaType);

            //audioMediaType.Set(MediaTypeAttributeKeys.MajorType, MediaTypeGuids.Audio);
            //audioMediaType.Set(MediaTypeAttributeKeys.Subtype, AudioFormatGuids.Float);
            //audioMediaType.Set(MediaTypeAttributeKeys.AudioSamplesPerSecond, options.SampleRate);
            //reader.SetCurrentMediaType(SourceReaderIndex.FirstAudioStream, audioMediaType);

            Video = new IVideoStream[]
            {
                new VideoStream(reader),
            };

            Audio = Array.Empty<IAudioStream>();

            Info = new(file, string.Empty, 0, Video[0].Info.Duration, TimeSpan.Zero, new());

        }

        public IVideoStream[] Video { get; }

        public IAudioStream[] Audio { get; }

        public MediaInfo Info { get; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
