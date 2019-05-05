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
            
            new MainWindow(args)
            {
                VSync = VSyncMode.On
            }.Run(20);

            return 0;
        }
    }
}
