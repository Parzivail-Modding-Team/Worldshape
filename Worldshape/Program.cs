using System.IO.Compression;
using System.Threading;
using MinecraftStructureLib.Core;
using OpenTK;
using Worldshape.Configuration;
using Worldshape.Logging;
using Worldshape.Window;

namespace Worldshape
{
    class Program
    {
	    internal static ConfigContainer Config;

        private static int Main(string[] args)
        {
            if (args.Length == 0)
                return -1;

            Thread.CurrentThread.Name = "main";

			Lumberjack.Init();

			Lumberjack.Debug("Loading configuration");
			Config = ConfigContainer.Load();

			if (Config.UnpackTextures)
			{
				Lumberjack.Debug("Unpacking textures");
				ZipFile.ExtractToDirectory("Resources/assets.zip", "Resources");
				Config.UnpackTextures = false;
				Config.Save();
			}

			Lumberjack.Debug("Loading window");
			new MainWindow(args)
            {
                VSync = VSyncMode.Off
            }.Run();

            return 0;
        }
    }
}
