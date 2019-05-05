using Worldshape.Graphics.Buffer;

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
        public void Prerender()
        {
            _vbi.Reset();

            for (var x = X * 16; x < X * 16 + 16; x++)
            for (var y = 0; y < 256; y++)
            for (var z = Z * 16; z < Z * 16 + 16; z++)
            {
                
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