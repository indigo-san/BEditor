#pragma once
#include "pch.h"

using namespace BEditor::Media;
using namespace BEditor::Media::Decoding;

namespace BEditor::Extensions::MF::Decoding {
	public ref class InputContainer : IInputContainer {
    private:
        initonly cli::array<IVideoStream^>^ _video;
        initonly cli::array<IAudioStream^>^ _audio;
        initonly MediaInfo^ _info;
        IMFSourceReader* _reader;

	public:
		InputContainer(String^ file, MediaOptions^ options);
		~InputContainer();


        property cli::array<IVideoStream^>^ Video{
            virtual cli::array<IVideoStream^>^ get() {
                return _video;
            }
        }

        property cli::array<IAudioStream^>^ Audio {
            virtual cli::array<IAudioStream^>^ get() {
                return _audio;
            }
        }

        property MediaInfo^ Info {
            virtual MediaInfo^ get() {
                return _info;
            }
        }
	};
}