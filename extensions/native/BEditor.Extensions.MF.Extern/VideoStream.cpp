#include "pch.h"


static void ConfigureVideoDecoder(IMFSourceReader* reader, GUID format) {
	IMFMediaType* mediaType;
	Throw(MFCreateMediaType(&mediaType));
	Throw(mediaType->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Video));
	Throw(mediaType->SetGUID(MF_MT_SUBTYPE, format));

	reader->SetCurrentMediaType(MF_SOURCE_READER_FIRST_VIDEO_STREAM, 0, mediaType);

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

void Capture(IMFSourceReader* reader, Image* img)
{
	DWORD flags;
	IMFSample* sample;
	DWORD streamIndex;
	LONGLONG llVideoTimeStamp = 0, llSampleDuration = 0;
	Throw(reader->ReadSample(MF_SOURCE_READER_FIRST_VIDEO_STREAM, 0, NULL, &flags, NULL, &sample));

	IMFMediaBuffer* buffer;
	DWORD bufLength;
	Throw(sample->GetBufferByIndex(0, &buffer));
	buffer->GetCurrentLength(&bufLength);

	BYTE* p;
	DWORD size;
	buffer->Lock(&p, NULL, &size);

	memcpy(img->data, p, bufLength);
	Throw(buffer->Unlock());

	buffer->Release();
	sample->Release();
}

VideoStream::VideoStream(IMFSourceReader* reader) {
	this->reader = reader;
	this->current = 0;
	ConfigureVideoDecoder(reader, MFVideoFormat_RGB32);
	const double timebase = 10.0 * 1000.0 * 1000.0;
	UINT32 width;
	UINT32 height;
	double fps;

	GetVideoInfo(reader, &width, &height, &fps);
	duration = GetDuration(reader);
	double duration = this->duration / timebase;

	info.codec = "";
	info.duration = duration;
	info.width = (int)width;
	info.height = (int)height;
	info.framenum = (int)(duration * fps);
	info.framerate = (int)fps;
}

VideoStream::~VideoStream() {

}

int VideoStream::TryGetFrame(long position, Image* image) {
	if (position > info.framenum)
	{
		return 0;
	}
	auto per = (float)position / info.framenum;
	SeekToKeyframe(reader, (int)(duration * per));

	Capture(reader, image);
	current = position;

	return 1;
}