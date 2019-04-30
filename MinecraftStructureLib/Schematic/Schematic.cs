using System;
using System.Collections;
using System.Collections.Generic;
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

		private readonly Dictionary<BlockPos, Block> _blocks = new Dictionary<BlockPos, Block>();
		private readonly Dictionary<BlockPos, TileEntity> _tiles = new Dictionary<BlockPos, TileEntity>();
		private readonly List<Entity> _entities = new List<Entity>();

		public override void Load(string filename)
		{
			var inputSchematic = new NBTFile(filename);
			var tag = new NbtTree(inputSchematic.GetDataInputStream()).Root;

			var bLower = tag["Blocks"].ToTagByteArray().Data;
			var bUpper = new byte[(bLower.Length >> 1) + 1];
			var bMetadata = new byte[bLower.Length];

			if (tag.ContainsKey("AddBlocks"))
				bUpper = tag["AddBlocks"].ToTagByteArray().Data;
			else if (tag.ContainsKey("Add"))
			{
				Console.WriteLine("Schematic contains deprecated tag \"Add\", use \"AddBlocks\" instead.");
				bUpper = tag["Add"].ToTagByteArray().Data;
			}

			if (tag.ContainsKey("Metadata")) bMetadata = tag["Metadata"].ToTagByteArray().Data;
			else if (tag.ContainsKey("Data")) bMetadata = tag["Data"].ToTagByteArray().Data;

			Length = tag["Length"].ToTagInt().Data;
			Width = tag["Width"].ToTagInt().Data;
			Height = tag["Height"].ToTagInt().Data;

			var teList = tag["TileEntities"].ToTagList();
			foreach (var genericTag in teList)
			{
				var teTag = genericTag.ToTagCompound();
				var x = teTag["x"].ToTagInt().Data;
				var y = teTag["y"].ToTagInt().Data;
				var z = teTag["z"].ToTagInt().Data;

				var pos = new BlockPos(x, y, z);
				_tiles.Add(new BlockPos(x, y, z), new TileEntity(pos, teTag));
			}

			for (var i = 0; i < bLower.Length; i++)
			{
				short id;
				if ((i & 1) == 1)
					id = (short)(((bUpper[i >> 1] & 0x0F) << 8) + (bLower[i] & 0xFF));
				else
					id = (short)(((bUpper[i >> 1] & 0xF0) << 4) + (bLower[i] & 0xFF));

				var pos = GetBlockPos(Length, Width, i);
				var metadata = bMetadata[i];
				_tiles.TryGetValue(pos, out var tile);

				_blocks.Add(pos, new Block(TranslateBlockId(id), metadata, tile?.Data));
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

		public override void Save(string filename)
		{
			throw new NotImplementedException();
		}

		public override Block this[int x, int y, int z]
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
	}
}
