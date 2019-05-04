using Substrate.Nbt;

namespace MinecraftStructureLib.Loader.StructureBlock
{
    internal class StructureBlockPaletteEntry
    {
        public readonly string Name;
        public readonly TagNodeCompound Props;

        public StructureBlockPaletteEntry(string name, TagNodeCompound props)
        {
            Name = name;
            Props = props;
        }

        protected bool Equals(StructureBlockPaletteEntry other)
        {
            return string.Equals(Name, other.Name) && Equals(Props, other.Props);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StructureBlockPaletteEntry) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Props != null ? Props.GetHashCode() : 0);
            }
        }

        public static bool operator ==(StructureBlockPaletteEntry left, StructureBlockPaletteEntry right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StructureBlockPaletteEntry left, StructureBlockPaletteEntry right)
        {
            return !Equals(left, right);
        }
    }
}