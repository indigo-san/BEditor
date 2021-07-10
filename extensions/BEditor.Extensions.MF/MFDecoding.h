#pragma once

using namespace System;
using namespace System::IO;
using namespace System::Collections::Generic;
using namespace System::Linq;
using namespace System::Text;
using namespace System::Threading::Tasks;

using namespace BEditor::Extensions::MF::Decoding;
using namespace BEditor::Media;
using namespace BEditor::Media::Decoding;

namespace BEditor::Extensions::MF {

	public ref class MFDecoding : IRegisterdDecoding
	{
	public:
		MFDecoding() {
		}

		property String^ Name {
			virtual String^ get() {
				return "MediaFoundation";
			}
		}

		virtual IInputContainer^ Open(String^ file, MediaOptions^ options);

		virtual bool IsSupported(String^ file) {
			return Enumerable::Contains(SupportExtensions(), Path::GetExtension(file));
		}

		virtual IEnumerable<String^>^ SupportExtensions() {
			cli::array<String^>^ strs = {
				".mp4",
				".m4a",
				".m4v",
				".mov",
				".avi",
				".3g2",
				".3gp",
				".3gpp",
				".asf",
				".wma",
				".wmv",
				".mp3",
				".wav",
				".aac",
				".adts",
			};

			return strs;
		}
	};
}