﻿using System.Runtime.InteropServices;

namespace Worldshape.Graphics.Primitive
{
    [StructLayout(LayoutKind.Explicit, Size = Size)]
    public struct Uv
    {
        public const int Size = 8;
        [FieldOffset(0)] public float U;
        [FieldOffset(4)] public float V;

        public Uv(float u, float v)
        {
            U = u;
            V = v;
        }
    }
}