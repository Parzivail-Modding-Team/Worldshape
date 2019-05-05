using Substrate;

namespace MinecraftStructureLib.Loader.Scarif
{
    public class ChunkBounds
    {
        public readonly int MaxX;
        public readonly int MaxY;
        public readonly int MaxZ;
        public readonly int MinX;

        public readonly int MinY;

        public readonly int MinZ;

        public ChunkBounds(int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
            MinZ = minZ;
            MaxZ = maxZ;
        }

        public ChunkBounds(string boundsStr)
        {
            var split = boundsStr.Split(':');
            MinX = int.Parse(split[0]);
            MinY = int.Parse(split[1]);
            MinZ = int.Parse(split[2]);
            MaxX = int.Parse(split[3]);
            MaxY = int.Parse(split[4]);
            MaxZ = int.Parse(split[5]);
        }

        public bool Contains(int x, int y, int z)
        {
            return x >= MinX && x <= MaxX && y >= MinY && y <= MaxY && z >= MinZ && z <= MaxZ;
        }

        public bool CoarseContains(ChunkRef chunk)
        {
            return chunk.X * 16 + 16 >= MinX &&
                   chunk.X * 16 <= MaxX &&
                   chunk.Z * 16 + 16 >= MinZ &&
                   chunk.Z * 16 <= MaxZ;
        }

        protected bool Equals(ChunkBounds other)
        {
            return MaxX == other.MaxX && MaxY == other.MaxY && MaxZ == other.MaxZ && MinX == other.MinX && MinY == other.MinY && MinZ == other.MinZ;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ChunkBounds) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = MaxX;
                hashCode = (hashCode * 397) ^ MaxY;
                hashCode = (hashCode * 397) ^ MaxZ;
                hashCode = (hashCode * 397) ^ MinX;
                hashCode = (hashCode * 397) ^ MinY;
                hashCode = (hashCode * 397) ^ MinZ;
                return hashCode;
            }
        }

        public static bool operator ==(ChunkBounds left, ChunkBounds right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ChunkBounds left, ChunkBounds right)
        {
            return !Equals(left, right);
        }
    }
}