using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using BEditor.Plugin;

using SharpDX.MediaFoundation;

namespace BEditor.Extensions.MediaFoundation
{
    public class Plugin : PluginObject
    {
        public Plugin(PluginConfig config) : base(config)
        {
            config.Application.Exit += Application_Exit;
        }

        public override string PluginName => "BEditor.Extensions.MediaFoundation";

        public override string Description => "";

        public override SettingRecord Settings { get; set; } = new();

        public override Guid Id { get; } = Guid.Parse("7B5EBA32-A582-4333-A920-FD8F68015432");

        public static void Register()
        {
            MediaManager.Startup();

            PluginBuilder.Configure<Plugin>()
                .With(new MFDecoding())
                //.With(new EncoderBuilder())
                //.With(new DecoderBuilder())
                .Register();
        }

        private void Application_Exit(object? sender, EventArgs e)
        {
            MediaManager.Shutdown();
        }
    }
}
