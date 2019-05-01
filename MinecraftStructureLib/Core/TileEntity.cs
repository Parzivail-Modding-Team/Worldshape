using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Substrate.Nbt;

namespace MinecraftStructureLib.Core
{
	public class TileEntity
	{
		public readonly BlockPos Position;
		public readonly TagNodeCompound Data;

		public TileEntity(BlockPos pos, TagNodeCompound data)
		{
			Position = pos;
			Data = data;
		}
	}
}
