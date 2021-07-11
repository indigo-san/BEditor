#ifndef PCH_H
#define PCH_H

// add headers that you want to pre-compile here
#include "framework.h"
#include "Image.h"
#include "AudioStream.h"
#include "VideoStream.h"
#include "InputContainer.h"

#define DLLExport(T) extern "C" __declspec(dllexport) T

#define Throw(RESULT_) { HRESULT result = RESULT_; if (FAILED(result)) { throw std::exception(std::to_string(result).c_str()); } }

#include "Extern.h"

#endif //PCH_H