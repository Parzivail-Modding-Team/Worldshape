using System;

namespace MinecraftStructureLib.Scarif
{
    [Flags]
    public enum BlockFlags
    {
        None = 0,
        Metadata = 0b1,
        Nbt = 0b10
    }
}