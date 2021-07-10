#include "pch.h"

using namespace BEditor::Extensions::MF;
using namespace BEditor::Extensions::MF::Decoding;
using namespace BEditor::Media;
using namespace BEditor::Media::Decoding;

IInputContainer^ MFDecoding::Open(String^ file, MediaOptions^ options) {
	return gcnew InputContainer(file, options);
}