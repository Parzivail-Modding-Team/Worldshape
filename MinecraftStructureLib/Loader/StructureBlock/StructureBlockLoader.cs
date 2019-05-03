using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinecraftStructureLib.Core;
using MinecraftStructureLib.Loader.Scarif;
using MinecraftStructureLib.Loader.Schematic;
using Substrate.Core;
using Substrate.Nbt;

namespace MinecraftStructureLib.Loader.StructureBlock
{
    class StructureBlockLoader : IStructureLoader
    {
        /// <inheritdoc />
        public bool CanLoad(string filename)
        {
            return Path.GetExtension(filename) == ".nbt";
        }

        /// <inheritdoc />
        public Structure Load(string filename)
        {
            var input = new NBTFile(filename);
            var nbt = new NbtTree(input.GetDataInputStream()).Root;

            var dataVersion = nbt["DataVersion"].ToTagInt().Data;
            var author = nbt.ContainsKey("author") ? nbt["author"].ToTagString().Data : null;
            
            var size = nbt["size"].ToTagList().Select(node => node.ToTagInt().Data).ToArray();
            var width = size[0];
            var height = size[1];
            var length = size[2];
            
            var palette = LoadPalette(nbt);
            var blocks = LoadBlocks(nbt);
            var entities = LoadEntities(nbt);

            return new StructureBlockStructure(author, width, height, length, palette, blocks, entities);
        }

        private Entity[] LoadEntities(TagNodeCompound nbt)
        {
            throw new NotImplementedException();
        }

        private Dictionary<BlockPos, Block> LoadBlocks(TagNodeCompound nbt)
        {
        }

        private TranslationMap LoadPalette(TagNodeCompound nbt)
        {
            throw new NotImplementedException();
        }
    }
}
