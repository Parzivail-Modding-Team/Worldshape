using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MinecraftStructureLib.Core;
using Substrate.Core;
using Substrate.Nbt;

namespace MinecraftStructureLib.Schematic
{
	public class Schematic : Structure
	{
		public int Length { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }

		private Block[] _blocks;
		private Entity[] _entities;

        /// <inheritdoc />
		public override void Load(string filename)
		{
			var inputSchematic = new NBTFile(filename);
			var tag = new NbtTree(inputSchematic.GetDataInputStream()).Root;

			var bLower = tag["Blocks"].ToTagByteArray().Data;
			var bUpper = new byte[(bLower.Length >> 1) + 1];

            if (tag.ContainsKey("AddBlocks"))
                bUpper = tag["AddBlocks"].ToTagByteArray().Data;
            else if (tag.ContainsKey("Add"))
            {
                Console.WriteLine("Schematic contains deprecated tag \"Add\", use \"AddBlocks\" instead.");
                bUpper = tag["Add"].ToTagByteArray().Data;
            }
            
            var bMetadata = new byte[bLower.Length];

            if (tag.ContainsKey("Metadata")) bMetadata = tag["Metadata"].ToTagByteArray().Data;
            else if (tag.ContainsKey("Data")) bMetadata = tag["Data"].ToTagByteArray().Data;
            
            var teList = tag["TileEntities"].ToTagList().Select(node => node.ToTagCompound());
            var eList = tag["Entities"].ToTagList().Select(node => node.ToTagCompound()).ToArray();

            Length = tag["Length"].ToTagInt().Data;
            Width = tag["Width"].ToTagInt().Data;
            Height = tag["Height"].ToTagInt().Data;

            var tiles = new Dictionary<BlockPos, TileEntity>();
			foreach (var teTag in teList)
			{
				var x = teTag["x"].ToTagInt().Data;
				var y = teTag["y"].ToTagInt().Data;
				var z = teTag["z"].ToTagInt().Data;

				var pos = new BlockPos(x, y, z);
                tiles.Add(pos, new TileEntity(pos, teTag));
			}

            _blocks = new Block[bLower.Length];
			for (var i = 0; i < bLower.Length; i++)
			{
				short id;
				if ((i & 1) == 1)
					id = (short)(((bUpper[i >> 1] & 0x0F) << 8) + (bLower[i] & 0xFF));
				else
					id = (short)(((bUpper[i >> 1] & 0xF0) << 4) + (bLower[i] & 0xFF));

				var pos = GetBlockPos(Length, Width, i);
				var metadata = bMetadata[i];
                tiles.TryGetValue(pos, out var tile);

				_blocks[i] = new Block(TranslateBlockId(id), metadata, new NbtTree(tile?.Data));
			}

            _entities = new Entity[eList.Length];
            for (var i = 0; i < eList.Length; i++)
            {
                var eTag = eList[i];
                var posList = eTag["Pos"]
                    .ToTagList()
                    .Select(node => node.ToTagDouble().Data)
                    .ToArray();
                _entities[i] = new Entity(posList[0], posList[1], posList[2], eTag);
            }
        }
        
		private string TranslateBlockId(int id)
		{
			return $"unknown:{id}";
		}

		private static BlockPos GetBlockPos(int length, int width, int index)
		{
			return new BlockPos((index % (width * length)) % width, index / (width * length), (index % (width * length)) / width);
		}
        
        /// <inheritdoc />
		public override void Save(string filename)
		{
			throw new NotImplementedException();
		}
        
        /// <inheritdoc />
		public override Block this[int x, int y, int z]
		{
			get => _blocks[(y * Length + z) * Width + x];
			set => _blocks[(y * Length + z) * Width + x] = value;
		}
	}
}
