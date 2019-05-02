using MinecraftStructureLib.Core;

namespace Worldshape
{
    class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length == 0)
                return -1;

            var structure = StructureLoader.Load(args[0]);

            return 0;
        }
    }
}
