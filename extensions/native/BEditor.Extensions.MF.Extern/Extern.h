#pragma once
#include "pch.h"

const char* Msg;

static void SetError(const char* msg) {
	Msg = msg;
}

DLLExport(const char*) GetError() {
	auto tmp = Msg;
	Msg = nullptr;
	return tmp;
}

DLLExport(int) Initialize() {
	CoInitialize(NULL);

	if (FAILED(MFStartup(MF_VERSION))) {
		return 0;
	}

	return 1;
}

DLLExport(int) Uninitialize() {
	CoUninitialize();

	if (FAILED(MFShutdown())) {
		return 0;
	}

	return 1;
}

DLLExport(InputContainer*) NewInputContainer(const char* file) {
	try {
		return new InputContainer(file);
	}
	catch (std::exception& exc) {
		SetError(exc.what());
		return nullptr;
	}
}

DLLExport(void) DeleteInputContainer(InputContainer* input) {
	delete input;
}

DLLExport(VideoStream*) GetVideoStream(InputContainer* input) {
	return input->video;
}

DLLExport(AudioStream*) GetAudioStream(InputContainer* input) {
	return input->audio;
}

DLLExport(int) VStream_TryGetFrame(VideoStream* stream, long position, Image* image) {
	try {
		return stream->TryGetFrame(position, image);
	}
	catch (std::exception& exc) {
		SetError(exc.what());
		return 0;
	}
}

DLLExport(VideoStreamInfo) VStream_GetInfo(VideoStream* stream) {
	return stream->info;
}