using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MinecraftStructureLib.Core;
using Substrate.Core;
using Substrate.Nbt;

namespace MinecraftStructureLib.Loader.Schematic
{
    internal class SchematicLoader : IStructureLoader
    {
        private string TranslateBlockId(int id)
        {
            return $"unknown:{id}";
        }

        private static BlockPos GetBlockPos(int length, int width, int index)
        {
            return new BlockPos(index % (width * length) % width, index / (width * length),
                index % (width * length) / width);
        }

        /// <inheritdoc />
        public bool CanLoad(string filename)
        {
            return Path.GetExtension(filename) == ".schematic";
        }

        /// <inheritdoc />
        public Structure Load(string filename)
        {
            var inputSchematic = new NBTFile(filename);
            var tag = new NbtTree(inputSchematic.GetDataInputStream()).Root;

            var bLower = tag["Blocks"].ToTagByteArray().Data;
            var bUpper = new byte[(bLower.Length >> 1) + 1];

            if (tag.ContainsKey("AddBlocks"))
            {
                bUpper = tag["AddBlocks"].ToTagByteArray().Data;
            }
            else if (tag.ContainsKey("Add"))
            {
                Console.WriteLine("Schematic contains deprecated tag \"Add\", use \"AddBlocks\" instead. Loading regardless.");
                var add = tag["Add"].ToTagByteArray().Data;
                for (var i = 0; i < bLower.Length; i++)
                {
                    if ((i & 1) == 1)
                        bUpper[i >> 1] |= (byte)(add[i] & 0x0F);
                    else
                        bUpper[i >> 1] |= (byte)((add[i] & 0x0F) << 4);
                }
            }

            var bMetadata = new byte[bLower.Length];

            if (tag.ContainsKey("Metadata")) bMetadata = tag["Metadata"].ToTagByteArray().Data;
            else if (tag.ContainsKey("Data")) bMetadata = tag["Data"].ToTagByteArray().Data;

            var teList = tag["TileEntities"].ToTagList().Select(node => node.ToTagCompound());
            var eList = tag["Entities"].ToTagList().Select(node => node.ToTagCompound()).ToArray();

            var length = tag["Length"].ToTagInt().Data;
            var width = tag["Width"].ToTagInt().Data;
            var height = tag["Height"].ToTagInt().Data;

            var tiles = new Dictionary<BlockPos, TileEntity>();
            foreach (var teTag in teList)
            {
                var x = teTag["x"].ToTagInt().Data;
                var y = teTag["y"].ToTagInt().Data;
                var z = teTag["z"].ToTagInt().Data;

                var pos = new BlockPos(x, y, z);
                tiles.Add(pos, new TileEntity(pos, teTag));
            }

            var blocks = new Block[bLower.Length];
            for (var i = 0; i < bLower.Length; i++)
            {
                short id;
                if ((i & 1) == 1)
                    id = (short)(((bUpper[i >> 1] & 0x0F) << 8) + (bLower[i] & 0xFF));
                else
                    id = (short)(((bUpper[i >> 1] & 0xF0) << 4) + (bLower[i] & 0xFF));

                var pos = GetBlockPos(length, width, i);
                var metadata = bMetadata[i];
                tiles.TryGetValue(pos, out var tile);

                blocks[i] = new Block(TranslateBlockId(id), metadata, new NbtTree(tile?.Data));
            }

            var entities = new Entity[eList.Length];
            for (var i = 0; i < eList.Length; i++)
            {
                var eTag = eList[i];
                var posList = eTag["Pos"]
                    .ToTagList()
                    .Select(node => node.ToTagDouble().Data)
                    .ToArray();
                entities[i] = new Entity(posList[0], posList[1], posList[2], eTag);
            }

            return new SchematicStructure(blocks, entities, width, height, length);
        }
    }
}