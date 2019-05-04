using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MinecraftStructureLib.Core;
using MinecraftStructureLib.Loader.Scarif;
using Substrate.Core;
using Substrate.Nbt;

namespace MinecraftStructureLib.Loader.Schematic
{
    internal class SchematicLoader : IStructureLoader
    {
        private static string TranslateBlockId(TranslationMap map, int id)
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
            var input = new NBTFile(filename);
            var nbt = new NbtTree(input.GetDataInputStream()).Root;

            var length = nbt["Length"].ToTagInt().Data;
            var width = nbt["Width"].ToTagInt().Data;
            var height = nbt["Height"].ToTagInt().Data;
            
            var palette = LoadPalette(nbt);
            var tiles = LoadTileEntities(nbt);
            var blocks = LoadBlocks(nbt, palette, length, width, tiles);
            var entities = LoadEntities(nbt);

            return new SchematicStructure(blocks, entities, palette, width, height, length);
        }

        private static TranslationMap LoadPalette(TagNodeCompound tag)
        {
            var map = new TranslationMap();

            if (tag.ContainsKey("SchematicaMapping")) // Schematica
            {
                foreach (var entry in tag["SchematicaMapping"].ToTagCompound())
                    map.Add(entry.Value.ToTagShort().Data, entry.Key);
            }
            else if (tag.ContainsKey("BlockIDs")) // MCEdit2
            {
                foreach (var entry in tag["BlockIDs"].ToTagCompound())
                    map.Add(short.Parse(entry.Key), entry.Value.ToTagString().Data);
            }

            return map;
        }

        private static Block[] LoadBlocks(TagNodeCompound tag, TranslationMap palette, int length, int width, Dictionary<BlockPos, TileEntity> tiles)
        {
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
                        bUpper[i >> 1] |= (byte) (add[i] & 0x0F);
                    else
                        bUpper[i >> 1] |= (byte) ((add[i] & 0x0F) << 4);
                }
            }

            var bMetadata = new byte[bLower.Length];

            if (tag.ContainsKey("Metadata")) bMetadata = tag["Metadata"].ToTagByteArray().Data;
            else if (tag.ContainsKey("Data")) bMetadata = tag["Data"].ToTagByteArray().Data;

            var blocks = new Block[bLower.Length];
            for (var i = 0; i < bLower.Length; i++)
            {
                short id;
                if ((i & 1) == 1)
                    id = (short) (((bUpper[i >> 1] & 0x0F) << 8) + (bLower[i] & 0xFF));
                else
                    id = (short) (((bUpper[i >> 1] & 0xF0) << 4) + (bLower[i] & 0xFF));

                var pos = GetBlockPos(length, width, i);
                var metadata = bMetadata[i];
                tiles.TryGetValue(pos, out var tile);

                blocks[i] = new Block(TranslateBlockId(palette, id), metadata, tile?.Data);
            }

            return blocks;
        }

        private static Dictionary<BlockPos, TileEntity> LoadTileEntities(TagNodeCompound tag)
        {
            var tiles = new Dictionary<BlockPos, TileEntity>();
            var teList = tag["TileEntities"].ToTagList().Select(node => node.ToTagCompound());
            foreach (var teTag in teList)
            {
                var x = teTag["x"].ToTagInt().Data;
                var y = teTag["y"].ToTagInt().Data;
                var z = teTag["z"].ToTagInt().Data;

                var pos = new BlockPos(x, y, z);
                tiles.Add(pos, new TileEntity(pos, teTag));
            }

            return tiles;
        }

        private static Entity[] LoadEntities(TagNodeCompound tag)
        {
            var eList = tag["Entities"].ToTagList().Select(node => node.ToTagCompound()).ToArray();

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

            return entities;
        }
    }
}