namespace MinecraftStructureLib.Scarif
{
    public class ChunkPosition
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
    }
}