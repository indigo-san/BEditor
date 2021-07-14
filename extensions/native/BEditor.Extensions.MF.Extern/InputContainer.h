#pragma once
#include "pch.h"

class InputContainer
{
public:
	VideoStream* video;
	AudioStream* audio;

	InputContainer(const char* file);
	~InputContainer();
private:
	IMFSourceResolver* pSourceResolver = NULL;
	IUnknown* uSource = NULL;
	IMFMediaSource* mediaFileSource = NULL;
	IMFAttributes* pVideoReaderAttributes = NULL;
	IMFSourceReader* reader = NULL;
	MF_OBJECT_TYPE ObjectType = MF_OBJECT_INVALID;
};
