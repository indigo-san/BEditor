#include "pch.h"

InputContainer::InputContainer(const char* file)
{
	try {
		IMFSourceReader* reader;
		wchar_t filename[4096] = { 0 };
		MultiByteToWideChar(0, 0, file, strlen(file), filename, strlen(file));
		Throw(MFCreateSourceReaderFromURL(filename, NULL, &reader));
		_reader = reader;

		video = new VideoStream(reader);

		audio = nullptr;
	}
	catch (std::exception& exc) {
		if (_reader != nullptr) {
			_reader->Release();
		}

		throw;
	}
}

InputContainer::~InputContainer()
{
	delete video;
	delete audio;
	_reader->Release();
}