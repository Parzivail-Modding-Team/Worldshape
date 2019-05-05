namespace MinecraftStructureLib.Loader.Scarif
{
    public struct ChunkPosition
    {
        public readonly int X;
        public readonly int Z;

        public ChunkPosition(int x, int z)
        {
            X = x;
            Z = z;
        }

        public override string ToString()
        {
            return $"({X},{Z})";
        }

        public bool Equals(ChunkPosition other)
        {
            return X == other.X && Z == other.Z;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ChunkPosition other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Z;
            }
        }

        public static bool operator ==(ChunkPosition left, ChunkPosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ChunkPosition left, ChunkPosition right)
        {
            return !left.Equals(right);
        }
    }
}