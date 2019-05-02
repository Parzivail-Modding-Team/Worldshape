using System;
using MinecraftStructureLib.Core;

namespace MinecraftStructureLib.Loader.Schematic
{
    public class SchematicStructure : Structure
    {
        public const string FileExtension = ".schematic";

        private readonly Block[] _blocks;
        private readonly Entity[] _entities;

        /// <inheritdoc />
        public override Block this[int x, int y, int z]
        {
            get => _blocks[(y * Length + z) * Width + x];
            set => _blocks[(y * Length + z) * Width + x] = value;
        }

        public SchematicStructure(Block[] blocks, Entity[] entities, int width, int height, int length)
        {
            _blocks = blocks;
            _entities = entities;
            Width = width;
            Height = height;
            Length = length;
        }

        /// <inheritdoc />
        public override void Save(string filename)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override string GetFileExtension()
        {
            return FileExtension;
        }
    }
}