using MinecraftStructureLib.Core;
using OpenTK;
using Worldshape.Window;

namespace Worldshape
{
    class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length == 0)
                return -1;

            var structure = StructureLoader.Load(args[0]);
            
            new MainWindow
            {
                VSync = VSyncMode.On
            }.Run(20);

            return 0;
        }
    }
}
