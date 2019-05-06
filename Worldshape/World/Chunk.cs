using MinecraftStructureLib.Core;
using Worldshape.Graphics.Buffer;
using Worldshape.Graphics.Primitive;
using Worldshape.Graphics.Texture;

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
        public void Prerender(Structure structure, TextureAtlas texAtlas)
        {
            _vbi.Reset();

            for (var x = X * 16; x < X * 16 + 16; x++)
                for (var y = 0; y < 256; y++)
                    for (var z = Z * 16; z < Z * 16 + 16; z++)
                    {
                        if (!structure.Contains(x, y, z) || structure.IsAir(x, y, z))
                            continue;

                        var block = structure[x, y, z];
                        var tex = texAtlas[block.Id];

                        var tc00 = new TexCoord(0, 0);
                        var tc10 = new TexCoord(1, 0);
                        var tc01 = new TexCoord(0, 1);
                        var tc11 = new TexCoord(1, 1);

                        if (tex != null)
                        {
                            tc00 = new TexCoord(tex.MinU, tex.MinV);
                            tc10 = new TexCoord(tex.MaxU, tex.MinV);
                            tc01 = new TexCoord(tex.MinU, tex.MaxV);
                            tc11 = new TexCoord(tex.MaxU, tex.MaxV);
                        }

                        if (structure.IsBorderingAir(x, y, z, FaceDir.PosX))
                        {
                            _vbi.Append(new Vertex(x + 1, y, z), new Vertex(FaceDir.PosX), tc00);
                            _vbi.Append(new Vertex(x + 1, y, z + 1), new Vertex(FaceDir.PosX), tc10);
                            _vbi.Append(new Vertex(x + 1, y + 1, z + 1), new Vertex(FaceDir.PosX), tc11);
                            _vbi.Append(new Vertex(x + 1, y + 1, z), new Vertex(FaceDir.PosX), tc01);
                        }

                        if (structure.IsBorderingAir(x, y, z, FaceDir.NegX))
                        {
                            _vbi.Append(new Vertex(x, y, z), new Vertex(FaceDir.NegX), tc00);
                            _vbi.Append(new Vertex(x, y, z + 1), new Vertex(FaceDir.NegX), tc10);
                            _vbi.Append(new Vertex(x, y + 1, z + 1), new Vertex(FaceDir.NegX), tc11);
                            _vbi.Append(new Vertex(x, y + 1, z), new Vertex(FaceDir.NegX), tc01);
                        }

                        if (structure.IsBorderingAir(x, y, z, FaceDir.PosY))
                        {
                            _vbi.Append(new Vertex(x, y + 1, z), new Vertex(FaceDir.PosY), tc00);
                            _vbi.Append(new Vertex(x + 1, y + 1, z), new Vertex(FaceDir.PosY), tc10);
                            _vbi.Append(new Vertex(x + 1, y + 1, z + 1), new Vertex(FaceDir.PosY), tc11);
                            _vbi.Append(new Vertex(x, y + 1, z + 1), new Vertex(FaceDir.PosY), tc01);
                        }

                        if (structure.IsBorderingAir(x, y, z, FaceDir.NegY))
                        {
                            _vbi.Append(new Vertex(x, y, z), new Vertex(FaceDir.NegY), tc00);
                            _vbi.Append(new Vertex(x + 1, y, z), new Vertex(FaceDir.NegY), tc10);
                            _vbi.Append(new Vertex(x + 1, y, z + 1), new Vertex(FaceDir.NegY), tc11);
                            _vbi.Append(new Vertex(x, y, z + 1), new Vertex(FaceDir.NegY), tc01);
                        }

                        if (structure.IsBorderingAir(x, y, z, FaceDir.PosZ))
                        {
                            _vbi.Append(new Vertex(x, y, z + 1), new Vertex(FaceDir.PosZ), tc00);
                            _vbi.Append(new Vertex(x + 1, y, z + 1), new Vertex(FaceDir.PosZ), tc10);
                            _vbi.Append(new Vertex(x + 1, y + 1, z + 1), new Vertex(FaceDir.PosZ), tc11);
                            _vbi.Append(new Vertex(x, y + 1, z + 1), new Vertex(FaceDir.PosZ), tc01);
                        }

                        if (structure.IsBorderingAir(x, y, z, FaceDir.NegZ))
                        {
                            _vbi.Append(new Vertex(x, y, z), new Vertex(FaceDir.NegZ), tc00);
                            _vbi.Append(new Vertex(x + 1, y, z), new Vertex(FaceDir.NegZ), tc10);
                            _vbi.Append(new Vertex(x + 1, y + 1, z), new Vertex(FaceDir.NegZ), tc11);
                            _vbi.Append(new Vertex(x, y + 1, z), new Vertex(FaceDir.NegZ), tc01);
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