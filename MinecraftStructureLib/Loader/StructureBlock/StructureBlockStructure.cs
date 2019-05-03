using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinecraftStructureLib.Core;
using MinecraftStructureLib.Loader.Scarif;

namespace MinecraftStructureLib.Loader.StructureBlock
{
    class StructureBlockStructure : Structure
    {
        private readonly string _author;
        private readonly int _width;
        private readonly int _height;
        private readonly int _length;
        private readonly TranslationMap _palette;
        private readonly Dictionary<BlockPos, Block> _blocks;
        private readonly Entity[] _entities;

        public StructureBlockStructure(string author, int width, int height, int length, TranslationMap palette, Dictionary<BlockPos, Block> blocks, Entity[] entities)
        {
            _author = author;
            _width = width;
            _height = height;
            _length = length;
            _palette = palette;
            _blocks = blocks;
            _entities = entities;
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Block this[int x, int y, int z]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Save(string filename)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override string GetFileExtension()
        {
            throw new NotImplementedException();
        }
    }
}
