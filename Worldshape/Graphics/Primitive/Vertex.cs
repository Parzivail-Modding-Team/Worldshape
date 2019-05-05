using System.Runtime.InteropServices;
using MinecraftStructureLib.Core;

namespace Worldshape.Graphics.Primitive
{
    [StructLayout(LayoutKind.Explicit, Size = Size)]
    public struct Vertex
    {
        public const int Size = 12;
        [FieldOffset(0)] public float X;
        [FieldOffset(4)] public float Y;
        [FieldOffset(8)] public float Z;

        public Vertex(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vertex(FaceDir dir)
        {
            X = dir.X;
            Y = dir.Y;
            Z = dir.Z;
        }
    }
}