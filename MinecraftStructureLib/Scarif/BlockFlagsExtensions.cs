using MinecraftStructureLib.Core;

namespace MinecraftStructureLib.Scarif
{
    public static class BlockFlagsExtensions
    {
        public static bool Has(this BlockFlags value, BlockFlags flag)
        {
            return (value & flag) != 0;
        }

        public static BlockFlags CreateFlags(this Block block)
        {
            return (block.Metadata == 0 ? BlockFlags.None : BlockFlags.Metadata) |
                   (block.Data == null ? BlockFlags.None : BlockFlags.Nbt);
        }
    }
}