using System.Collections.Generic;
using Worldshape.Graphics.Primitive;

namespace Worldshape.Graphics.Buffer
{
    public class ChunkBuffer
    {
        public object LockHandle;
        public List<Vertex> VertexBuffer;
        public List<Vertex> NormalBuffer;
        public List<TexCoord> TexCoordBuffer;
        public List<short> IndexBuffer;
        public short Length;

        public ChunkBuffer()
        {
            LockHandle = new object();
            VertexBuffer = new List<Vertex>();
            NormalBuffer = new List<Vertex>();
            TexCoordBuffer = new List<TexCoord>();
            IndexBuffer = new List<short>();
            Length = 0;
        }

        public void Reset()
        {
            lock (LockHandle)
            {
                VertexBuffer.Clear();
                NormalBuffer.Clear();
                TexCoordBuffer.Clear();
                IndexBuffer.Clear();
            }
        }

        public short Append(Vertex pos, Vertex normal, TexCoord texCoord)
        {
            lock (LockHandle)
            {
                VertexBuffer.Add(pos);
                NormalBuffer.Add(normal);
                TexCoordBuffer.Add(texCoord);
                IndexBuffer.Add(Length);
                Length++;
                return (short) (Length - 1);
            }
        }
    }
}