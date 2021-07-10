#pragma once

using namespace System;
using namespace BEditor::Plugin;

namespace BEditor::Extensions::MF {
	public ref class Plugin : PluginObject {
	private:
		SettingRecord^ _settings = gcnew SettingRecord();

	public:
		Plugin(PluginConfig^ config)
			: PluginObject(config)
		{
			config->Application->Exit += gcnew EventHandler(&Exit);
		}

		property String^ PluginName {
			virtual String^ get() override {
				return "BEditor.Extensions.MF";
			}
		}

		property String^ Description {
			virtual String^ get() override {
				return "";
			}
		}

		property Guid Id {
			virtual Guid get() override {
				return Guid::Parse("7B5EBA32-A582-4333-A920-FD8F68015432");
			}
		}

		property SettingRecord^ Settings {
			virtual SettingRecord^ get() override {
				return _settings;
			}
			virtual void set(SettingRecord^ value) override {
				_settings = value;
			}
		}

		static void Register();
		static void Exit(Object^ sender, EventArgs^ e);
	};
}