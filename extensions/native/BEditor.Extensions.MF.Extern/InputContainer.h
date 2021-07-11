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
	IMFSourceReader* _reader;
};
