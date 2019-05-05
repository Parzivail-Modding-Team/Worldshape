using Substrate.Nbt;

namespace MinecraftStructureLib.Core
{
    public class Block
    {
        public readonly TagNodeCompound BlockState;
        public readonly TagNodeCompound Data;
        public readonly string Id;
        public readonly int Metadata;

        public Block(string id, int metadata = 0, TagNodeCompound data = null)
        {
            Id = id;
            Metadata = metadata;
            Data = data;
        }

        public Block(string id, TagNodeCompound blockState, TagNodeCompound data = null)
        {
            Id = id;
            BlockState = blockState;
            Data = data;
        }

        protected bool Equals(Block other)
        {
            return Equals(BlockState, other.BlockState) && Equals(Data, other.Data) && string.Equals(Id, other.Id) && Metadata == other.Metadata;
        }

        public override string ToString()
        {
            return $"{Id}.{Metadata}";
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Block) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (BlockState != null ? BlockState.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Data != null ? Data.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Metadata;
                return hashCode;
            }
        }

        public static bool operator ==(Block left, Block right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Block left, Block right)
        {
            return !Equals(left, right);
        }
    }
}