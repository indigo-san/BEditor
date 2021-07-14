#include "pch.h"

InputContainer::InputContainer(const char* file)
{
	try {
		wchar_t filename[4096] = { 0 };
		size_t length = strlen(file);
		MultiByteToWideChar(0, 0, file, length, filename, length);

		Throw(MFCreateSourceResolver(&pSourceResolver));
		Throw(pSourceResolver->CreateObjectFromURL(
			filename,
			MF_RESOLUTION_MEDIASOURCE,
			NULL,
			&ObjectType,
			&uSource));

		Throw(uSource->QueryInterface(IID_PPV_ARGS(&mediaFileSource)));

		Throw(MFCreateAttributes(&pVideoReaderAttributes, 2));

		Throw(pVideoReaderAttributes->SetGUID(MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID));

		Throw(pVideoReaderAttributes->SetUINT32(MF_SOURCE_READER_ENABLE_VIDEO_PROCESSING, 1));

		Throw(pVideoReaderAttributes->SetGUID(MF_MT_SUBTYPE, MFVideoFormat_RGB32));

		Throw(MFCreateSourceReaderFromMediaSource(mediaFileSource, pVideoReaderAttributes, &reader));

		video = new VideoStream(reader);

		audio = nullptr;
	}
	catch (std::exception& exc) {
		if (reader != nullptr) {
			reader->Release();
		}

		throw;
	}
}

InputContainer::~InputContainer()
{
	delete video;
	delete audio;

	pSourceResolver->Release();
	uSource->Release();
	mediaFileSource->Release();
	pVideoReaderAttributes->Release();
	pSourceResolver->Release();
	reader->Release();
}