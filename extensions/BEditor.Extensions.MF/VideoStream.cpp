#include "pch.h"
#include <vector>

using namespace System;

using namespace BEditor::Drawing;
using namespace BEditor::Drawing::Pixel;
using namespace BEditor::Media;
using namespace BEditor::Media::Decoding;
using namespace BEditor::Extensions::MF::Decoding;

#define Throw(RESULT) { auto result = RESULT; if (!result) { auto exc = gcnew Exception(); exc->HResult = result; throw exc; } }

static void ConfigureVideoDecoder(IMFSourceReader* reader, GUID format) {
	IMFMediaType* mediaType;
	Throw(MFCreateMediaType(&mediaType));
	Throw(mediaType->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Video));
	Throw(mediaType->SetGUID(MF_MT_SUBTYPE, format));

	Throw(reader->SetCurrentMediaType(MF_SOURCE_READER_FIRST_VIDEO_STREAM, 0, mediaType));

	mediaType->Release();
}

static void GetVideoInfo(IMFSourceReader* reader, UINT32* width, UINT32* height, double* fps) {
	IMFMediaType* mediaType;
	Throw(reader->GetCurrentMediaType(MF_SOURCE_READER_FIRST_VIDEO_STREAM, &mediaType));

	Throw(MFGetAttributeSize(mediaType, MF_MT_FRAME_SIZE, width, height));

	UINT32 nume, denom;
	Throw(MFGetAttributeRatio(mediaType, MF_MT_FRAME_RATE, &nume, &denom));
	*fps = (double)nume / denom;

	mediaType->Release();
}

static void SeekToKeyframe(IMFSourceReader* reader, int _100ns) {
	PROPVARIANT var;
	Throw(InitPropVariantFromInt64(_100ns, &var));
	Throw(reader->SetCurrentPosition(GUID_NULL, var));
	Throw(PropVariantClear(&var));
}

static LONGLONG GetDuration(IMFSourceReader* reader) {
	PROPVARIANT var;
	Throw(reader->GetPresentationAttribute(MF_SOURCE_READER_MEDIASOURCE, MF_PD_DURATION, &var));
	LONGLONG duration;
	Throw(PropVariantToInt64(var, &duration));
	Throw(PropVariantClear(&var));

	return duration;
}

void Capture(IMFSourceReader* reader, Image<VideoStream::RGB32>^ img)
{
	DWORD flags;
	IMFSample* sample;
	Throw(reader->ReadSample(MF_SOURCE_READER_FIRST_VIDEO_STREAM, 0, NULL, &flags, NULL, &sample));

	IMFMediaBuffer* buffer;
	Throw(sample->GetBufferByIndex(0, &buffer));

	BYTE* p;
	DWORD size;
	buffer->Lock(&p, NULL, &size);
	auto ptr = img->GetPointer();
	memcpy(ptr, p, size);
	Throw(buffer->Unlock());

	buffer->Release();
	sample->Release();
}

VideoStream::VideoStream(IMFSourceReader* reader) {
	_reader = reader;
	ConfigureVideoDecoder(reader, MFVideoFormat_RGB32);
	const double timebase = 10 * 1000 * 1000;
	UINT32 width;
	UINT32 height;
	double fps;

	GetVideoInfo(reader, &width, &height, &fps);
	_duration = GetDuration(reader);
	double duration = _duration / timebase;

	_info = gcnew VideoStreamInfo(
		"",
		BEditor::Media::MediaType::Video,
		TimeSpan::FromSeconds(duration),
		Size((int)width, (int)height),
		(int)(duration * fps),
		(int)fps);
}

VideoStream::~VideoStream() {

}

bool VideoStream::TryGetFrameCore(long position, [System::Runtime::InteropServices::Out] Image<BGRA32>^% image) {
	if (position > Info->NumberOfFrames)
	{
		image = nullptr;
		return false;
	}
	_current = position;
	auto per = (float)position / Info->NumberOfFrames;
	SeekToKeyframe(_reader, (int)(_duration * per));

	auto img = gcnew Image<VideoStream::RGB32>(Info->FrameSize.Width, Info->FrameSize.Height, RGB32());
	Capture(_reader, img);

	image = Image::Convert<RGB32, BGRA32>(img);
	return true;
}

Image<BGRA32>^ VideoStream::GetFrame(TimeSpan time) {
	Image<BGRA32>^ img = nullptr;
	if (!TryGetFrame(time, img)) throw gcnew EndOfStreamException();
	return img;
}

Image<BGRA32>^ VideoStream::GetNextFrame() {
	Image<BGRA32>^ img = nullptr;
	if (!TryGetNextFrame(img)) throw gcnew EndOfStreamException();
	return img;
}

bool VideoStream::TryGetFrame(TimeSpan time, [System::Runtime::InteropServices::Out] Image<BGRA32>^% image) {
	return TryGetFrameCore((long)(time / Info->Duration * Info->NumberOfFrames), image);
}

bool VideoStream::TryGetNextFrame([System::Runtime::InteropServices::Out] Image<BGRA32>^% image) {
	return TryGetFrameCore(_current + 1, image);
}