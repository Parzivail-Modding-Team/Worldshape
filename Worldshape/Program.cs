using System.IO.Compression;
using MinecraftStructureLib.Core;
using OpenTK;
using Worldshape.Configuration;
using Worldshape.Window;

namespace Worldshape
{
    class Program
    {
	    internal static Config Config;

        private static int Main(string[] args)
        {
            if (args.Length == 0)
                return -1;

			Config = Config.Load();

			if (Config.UnpackTextures)
			{
				ZipFile.ExtractToDirectory("Resources/assets.zip", "Resources");
				Config.UnpackTextures = false;
				Config.Save();
			}

			new MainWindow(args)
            {
                VSync = VSyncMode.On
            }.Run(20);

            return 0;
        }
    }
}
