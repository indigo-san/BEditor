#pragma once
#include "pch.h"

typedef	struct VideoStreamInfo
{
	// コーデック名
	const char* codec;
	// 秒数
	double duration;
	// 幅
	int width;
	// 高さ
	int height;
	// フレーム数
	int framenum;
	// フレームレート
	int framerate;
} VideoStreamInfo;

class VideoStream
{
public:
	VideoStreamInfo info;

	VideoStream(IMFSourceReader* reader);
	~VideoStream();

	int TryGetFrame(long position, Image* image);
private:
	IMFSourceReader* reader;
	long duration;
	long current;
};