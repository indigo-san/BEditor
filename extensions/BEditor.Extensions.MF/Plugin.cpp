#include "pch.h"

using namespace System;
using namespace BEditor::Plugin;
using namespace BEditor::Extensions::MF;
using namespace BEditor::Media;
using namespace BEditor::Media::Decoding;

void Plugin::Register() {
	CoInitialize(NULL);
	MFStartup(MF_VERSION);
	PluginBuilder::Configure<Plugin^>()
		->With(gcnew MFDecoding())
		->Register();
}

void Plugin::Exit(Object^ sender, EventArgs^ e) {
	MFShutdown();
}