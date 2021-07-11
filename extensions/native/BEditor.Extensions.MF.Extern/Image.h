#pragma once
#include "pch.h"

typedef struct Rgb32 {
	unsigned char r, g, b, reserve;
} Rgb32;

typedef struct Image {
	int width;
	int height;
	Rgb32* data;
} Image;