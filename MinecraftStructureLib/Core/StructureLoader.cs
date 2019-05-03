using System;
using System.Collections.Generic;
using System.Linq;
using MinecraftStructureLib.Loader.Scarif;
using MinecraftStructureLib.Loader.Schematic;
using MinecraftStructureLib.Loader.StructureBlock;

namespace MinecraftStructureLib.Core
{
    public class StructureLoader
    {
        private static readonly Dictionary<Type, IStructureLoader> Loaders = new Dictionary<Type, IStructureLoader>
        {
            {typeof(ScarifStructure), new ScarifLoader()},
            {typeof(SchematicStructure), new SchematicLoader()},
            {typeof(StructureBlockStructure), new StructureBlockLoader()}
        };

        public static void Register(Type type, IStructureLoader loader)
        {
            Loaders.Add(type, loader);
        }

        public static void Unregister(Type type)
        {
            Loaders.Remove(type);
        }

        public static IStructureLoader GetLoader(string filename)
        {
            return Loaders.FirstOrDefault(loader => loader.Value.CanLoad(filename)).Value;
        }

        public static IStructureLoader GetLoader(Type type)
        {
            return Loaders.TryGetValue(type, out var loader) ? loader : null;
        }

        public static Structure Load(Type type, string filename)
        {
            var loader = GetLoader(type);
            try
            {
                return loader?.Load(filename);
            }
            catch (Exception)
            {
                return null;
            }
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