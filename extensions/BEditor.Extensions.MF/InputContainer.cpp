#include "pch.h"

using namespace BEditor::Extensions::MF::Decoding;

#define Throw(RESULT) { auto result = RESULT; if (!result) { auto exc = gcnew Exception(); exc->HResult = result; throw exc; } }

InputContainer::InputContainer(String^ file, MediaOptions^ options) {
	IMFSourceReader* reader;
	pin_ptr<const wchar_t> wch = PtrToStringChars(file);
	MFCreateSourceReaderFromURL(wch, NULL, &reader);
	_reader = reader;


    _video = gcnew cli::array<VideoStream^>
    {
        gcnew VideoStream(reader)
    };

    _audio = System::Array::Empty<IAudioStream^>();

    _info = gcnew MediaInfo(file, String::Empty, 0, _video[0]->Info->Duration, TimeSpan::Zero, gcnew ContainerMetadata());
}

InputContainer::~InputContainer() {

}