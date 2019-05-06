using System;
using MinecraftStructureLib.Core;
using MinecraftStructureLib.Core.Translation;
using MinecraftStructureLib.Loader.Scarif;

namespace MinecraftStructureLib.Loader.Schematic
{
    public class SchematicStructure : Structure
    {
        public const string FileExtension = ".schematic";

        private readonly Block[] _blocks;
        private readonly Entity[] _entities;
        private readonly TranslationMap _palette;

        /// <inheritdoc />
        public override Block this[int x, int y, int z]
        {
            get => _blocks[(y * Length + z) * Width + (Width - (x + 1))];
            set => _blocks[(y * Length + z) * Width + (Width - (x + 1))] = value;
        }

        public SchematicStructure(Block[] blocks, Entity[] entities, TranslationMap palette, int width, int height, int length)
        {
            _blocks = blocks;
            _entities = entities;
            _palette = palette;
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