using MinecraftStructureLib.Core;
using Worldshape.Graphics.Buffer;
using Worldshape.Graphics.Primitive;

namespace Worldshape.World
{
    public class Chunk
    {
        public int X { get; }
        public int Z { get; }

        private readonly VertexBuffer _vbo = new VertexBuffer();
        private readonly ChunkBuffer _vbi = new ChunkBuffer();

        public Chunk(int x, int z)
        {
            X = x;
            Z = z;
        }

        /// <summary>
        /// Initializes the VBO but does not modify graphics state
        /// </summary>
        public void Prerender(Structure structure)
        {
            _vbi.Reset();

            for (var x = X * 16; x < X * 16 + 16; x++)
                for (var y = 0; y < 256; y++)
                    for (var z = Z * 16; z < Z * 16 + 16; z++)
                    {
                        if (!structure.Contains(x, y, z) || structure.IsAir(x, y, z))
                            continue;

                        if (structure.IsBorderingAir(x, y, z, FaceDir.PosX))
                        {
                            _vbi.Append(new Vertex(x + 1, y, z), new Vertex(FaceDir.PosX), new TexCoord(0, 0));
                            _vbi.Append(new Vertex(x + 1, y, z + 1), new Vertex(FaceDir.PosX), new TexCoord(1, 0));
                            _vbi.Append(new Vertex(x + 1, y + 1, z + 1), new Vertex(FaceDir.PosX), new TexCoord(1, 1));
                            _vbi.Append(new Vertex(x + 1, y + 1, z), new Vertex(FaceDir.PosX), new TexCoord(0, 1));
                        }

                        if (structure.IsBorderingAir(x, y, z, FaceDir.NegX))
                        {
                            _vbi.Append(new Vertex(x, y, z), new Vertex(FaceDir.NegX), new TexCoord(0, 0));
                            _vbi.Append(new Vertex(x, y, z + 1), new Vertex(FaceDir.NegX), new TexCoord(1, 0));
                            _vbi.Append(new Vertex(x, y + 1, z + 1), new Vertex(FaceDir.NegX), new TexCoord(1, 1));
                            _vbi.Append(new Vertex(x, y + 1, z), new Vertex(FaceDir.NegX), new TexCoord(0, 1));
                        }

                        if (structure.IsBorderingAir(x, y, z, FaceDir.PosY))
                        {
                            _vbi.Append(new Vertex(x, y + 1, z), new Vertex(FaceDir.PosY), new TexCoord(0, 0));
                            _vbi.Append(new Vertex(x + 1, y + 1, z), new Vertex(FaceDir.PosY), new TexCoord(1, 0));
                            _vbi.Append(new Vertex(x + 1, y + 1, z + 1), new Vertex(FaceDir.PosY), new TexCoord(1, 1));
                            _vbi.Append(new Vertex(x, y + 1, z + 1), new Vertex(FaceDir.PosY), new TexCoord(0, 1));
                        }

                        if (structure.IsBorderingAir(x, y, z, FaceDir.NegY))
                        {
                            _vbi.Append(new Vertex(x, y, z), new Vertex(FaceDir.NegY), new TexCoord(0, 0));
                            _vbi.Append(new Vertex(x + 1, y, z), new Vertex(FaceDir.NegY), new TexCoord(1, 0));
                            _vbi.Append(new Vertex(x + 1, y, z + 1), new Vertex(FaceDir.NegY), new TexCoord(1, 1));
                            _vbi.Append(new Vertex(x, y, z + 1), new Vertex(FaceDir.NegY), new TexCoord(0, 1));
                        }

                        if (structure.IsBorderingAir(x, y, z, FaceDir.PosZ))
                        {
                            _vbi.Append(new Vertex(x, y, z + 1), new Vertex(FaceDir.PosZ), new TexCoord(0, 0));
                            _vbi.Append(new Vertex(x + 1, y, z + 1), new Vertex(FaceDir.PosZ), new TexCoord(1, 0));
                            _vbi.Append(new Vertex(x + 1, y + 1, z + 1), new Vertex(FaceDir.PosZ), new TexCoord(1, 1));
                            _vbi.Append(new Vertex(x, y + 1, z + 1), new Vertex(FaceDir.PosZ), new TexCoord(0, 1));
                        }

                        if (structure.IsBorderingAir(x, y, z, FaceDir.NegZ))
                        {
                            _vbi.Append(new Vertex(x, y, z), new Vertex(FaceDir.NegZ), new TexCoord(0, 0));
                            _vbi.Append(new Vertex(x + 1, y, z), new Vertex(FaceDir.NegZ), new TexCoord(1, 0));
                            _vbi.Append(new Vertex(x + 1, y + 1, z), new Vertex(FaceDir.NegZ), new TexCoord(1, 1));
                            _vbi.Append(new Vertex(x, y + 1, z), new Vertex(FaceDir.NegZ), new TexCoord(0, 1));
                        }
                    }
        }

        /// <summary>
        /// Uploads the VBO to OpenGL
        /// </summary>
        public void Render()
        {
            _vbo.InitializeVbo(_vbi);
            _vbi.Reset();
        }

        /// <summary>
        /// Renders the VBO
        /// </summary>
        public void Draw()
        {
            if (!_vbo.Initialized)
                return;

            _vbo.Render();
        }
    }
}