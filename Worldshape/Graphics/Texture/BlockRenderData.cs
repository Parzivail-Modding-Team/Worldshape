using System.Collections.Generic;
using Worldshape.Configuration;

namespace Worldshape.Graphics.Texture
{
    public class BlockRenderData
    {
        public BlockProperties Properties { get; }
        public List<Texture> Textures { get; }

        public BlockRenderData(BlockProperties properties)
        {
            Properties = properties;
            Textures = new List<Texture>();
        }
    }
}