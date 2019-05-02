using Substrate.Nbt;

namespace MinecraftStructureLib.Core
{
    public class TileEntity
    {
        public readonly TagNodeCompound Data;
        public readonly BlockPos Position;

        public TileEntity(BlockPos pos, TagNodeCompound data)
        {
            Position = pos;
            Data = data;
        }
    }
}