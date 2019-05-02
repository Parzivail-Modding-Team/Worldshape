using System;
using System.Collections.Generic;
using System.Linq;
using MinecraftStructureLib.Loader.Scarif;
using MinecraftStructureLib.Loader.Schematic;

namespace MinecraftStructureLib.Core
{
    public class StructureLoader
    {
        private static readonly List<IStructureLoader> Loaders = new List<IStructureLoader>
        {
            new ScarifLoader(),
            new SchematicLoader()
        };

        public static void Register(IStructureLoader loader)
        {
            Loaders.Add(loader);
        }

        public static void Unregister(IStructureLoader loader)
        {
            Loaders.Remove(loader);
        }

        public static IStructureLoader GetLoader(string filename)
        {
            return Loaders.FirstOrDefault(loader => loader.CanLoad(filename));
        }

        public static Structure Load(string filename)
        {
            var loader = GetLoader(filename);
            try
            {
                return loader?.Load(filename);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}