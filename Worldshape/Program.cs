using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScarifLib;

namespace Worldshape
{
    class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length == 0)
                return -1;

            var structure = ScarifStructure.Load(args[0]);

            return 0;
        }
    }
}
