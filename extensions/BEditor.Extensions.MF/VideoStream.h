#pragma once
#include "pch.h"

using namespace System;

using namespace BEditor::Drawing;
using namespace BEditor::Drawing::Pixel;
using namespace BEditor::Media;
using namespace BEditor::Media::Decoding;

namespace BEditor::Extensions::MF::Decoding {
	public ref class VideoStream : IVideoStream {
	private:
		initonly VideoStreamInfo^ _info;
		initonly IMFSourceReader* _reader;
		// Nano Seconds
		initonly long _duration;
		// åªç›ÇÃÉtÉåÅ[ÉÄ
		long _current;

		bool TryGetFrameCore(long position, [System::Runtime::InteropServices::Out] Image<BGRA32>^% image);
	protected:
	public:
		VideoStream(IMFSourceReader* reader);
		~VideoStream();

		property VideoStreamInfo^ Info {
			virtual VideoStreamInfo^ get() {
				return _info;
			}
		}

		property StreamInfo^ Info1 {
			virtual StreamInfo^ get() = IMediaStream::Info::get {
				return _info;
			}
		}

		virtual Image<BGRA32>^ GetFrame(TimeSpan time);

		virtual Image<BGRA32>^ GetNextFrame();

		virtual bool TryGetFrame(TimeSpan time, [System::Runtime::InteropServices::Out] Image<BGRA32>^% image);

		virtual bool TryGetNextFrame([System::Runtime::InteropServices::Out] Image<BGRA32>^% image);

		value class RGB32 : IPixel<RGB32>, IPixelConvertable<BGRA32>
		{
		public:
			Byte R;
			Byte G;
			Byte B;

			virtual RGB32 Add(RGB32 foreground) {
				return RGB32();
			}

			virtual RGB32 Blend(RGB32 foreground) {
				return RGB32();
			}

			virtual void ConvertFrom(BGRA32 src) {
				R = src.R;
				G = src.G;
				B = src.B;
			}

			virtual void ConvertTo(BGRA32% dst) {
				dst = BGRA32(R, G, B, 255);
			}

			virtual RGB32 Subtract(RGB32 foreground) {
				return RGB32();
			}
		};
	};
}