using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            var blocks = LoadBlocks(nbt, palette);
            var entities = LoadEntities(nbt);

            return new StructureBlockStructure(author, width, height, length, blocks, entities);
        }

        private static Entity[] LoadEntities(TagNodeCompound nbt)
        {
            var entityList = nbt["entities"].ToTagList();
            var entities = new Entity[entityList.Count];

            for (var i = 0; i < entityList.Count; i++)
            {
                var tag = entityList[i].ToTagCompound();
                var pos = tag["pos"].ToTagList().Select(node => node.ToTagDouble().Data).ToArray();
                var entityNbt = tag["nbt"].ToTagCompound();
                entities[i] = new Entity(pos[0], pos[1], pos[2], entityNbt);
            }

            return entities;
        }

        private static Dictionary<BlockPos, Block> LoadBlocks(TagNodeCompound nbt, StructureBlockPaletteEntry[] palette)
        {
            var blockList = nbt["blocks"].ToTagList();
            var blocks = new Dictionary<BlockPos, Block>();

            foreach (var entry in blockList)
            {
                var tag = entry.ToTagCompound();
                var pos = tag["pos"].ToTagList().Select(node => node.ToTagInt().Data).ToArray();
                var stateIdx = (short)tag["state"].ToTagInt().Data;
                var state = palette[stateIdx];
                var blockNbt = tag.ContainsKey("nbt") ? tag["nbt"].ToTagCompound() : null;
                blocks.Add(new BlockPos(pos[0], pos[1], pos[2]), new Block(state.Name, state.Props, blockNbt));
            }

            return blocks;
        }

        private static StructureBlockPaletteEntry[] LoadPalette(TagNodeCompound nbt)
        {
            var paletteList = nbt["palettes"].ToTagList();
            var paletteEntries = new StructureBlockPaletteEntry[paletteList.Count];

            for (var i = 0; i < paletteList.Count; i++)
            {
                var tag = paletteList[i].ToTagCompound();
                var name = tag["Name"].ToTagString().Data;
                var props = tag["Properties"].ToTagCompound();
                paletteEntries[i] = new StructureBlockPaletteEntry(name, props);
            }

            return paletteEntries;
        }
    }
}
