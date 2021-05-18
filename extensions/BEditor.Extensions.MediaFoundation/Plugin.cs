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
        }

        public override string PluginName => "BEditor.Extensions.MediaFoundation";

        public override string Description => "";

        public override SettingRecord Settings { get; set; } = new();

        public static void Register(string[] args)
        {
            MediaManager.Startup();

            PluginBuilder.Configure<Plugin>()
                //.With(new EncoderBuilder())
                //.With(new DecoderBuilder())
                .Register();
        }
    }
}
