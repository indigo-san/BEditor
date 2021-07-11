using System;

using BEditor.Plugin;

namespace BEditor.Extensions.MF
{
    public class Plugin : PluginObject
    {
        public Plugin(PluginConfig config)
            : base(config)
        {
            config.Application.Exit += Application_Exit;
        }

        public override string PluginName => "BEditor.Extensions.MF";

        public override string Description => "";

        public override SettingRecord Settings { get; set; } = new();

        public override Guid Id { get; } = Guid.Parse("7B5EBA32-A582-4333-A920-FD8F68015432");

        public static void Register()
        {
            if (OperatingSystem.IsWindows())
            {
                if (NativeMethods.Initialize() is not 0)
                {
                    PluginBuilder.Configure<Plugin>()
                        .Register();
                }
                else
                {
                    var msg = NativeMethods.GetError();
                }
            }
        }

        private void Application_Exit(object? sender, EventArgs e)
        {
            NativeMethods.Uninitialize();
        }
    }
}
