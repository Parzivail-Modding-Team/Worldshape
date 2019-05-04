namespace MinecraftStructureLib.Core
{
    public struct BlockPos
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        public BlockPos(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public bool Equals(BlockPos other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is BlockPos other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ Z;
                return hashCode;
            }
        }

        public static bool operator ==(BlockPos left, BlockPos right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BlockPos left, BlockPos right)
        {
            return !left.Equals(right);
        }
    }
}