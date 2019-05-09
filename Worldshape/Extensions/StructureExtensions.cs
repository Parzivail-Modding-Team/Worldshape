using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinecraftStructureLib.Core;
using Worldshape.Graphics.Texture;

namespace Worldshape.Extensions
{
    static class StructureExtensions
    {
        public static bool IsBorderingTransparent(this Structure structure, int x, int y, int z, FaceDir face, RenderAtlas renderAtlas)
        {
            switch (face.Facing)
            {
                case FaceDir.Dir.PosX:
                    if (x == structure.Width - 1)
                        return true;
                    break;
                case FaceDir.Dir.NegX:
                    if (x == 0)
                        return true;
                    break;
                case FaceDir.Dir.PosY:
                    if (y == structure.Height - 1)
                        return true;
                    break;
                case FaceDir.Dir.NegY:
                    if (y == 0)
                        return true;
                    break;
                case FaceDir.Dir.PosZ:
                    if (z == structure.Length - 1)
                        return true;
                    break;
                case FaceDir.Dir.NegZ:
                    if (z == 0)
                        return true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return structure.IsTransparent(x + face.X, y + face.Y, z + face.Z, renderAtlas);
        }

        public static bool IsBorderingTransparent(this Structure structure, int x, int y, int z, RenderAtlas renderAtlas)
        {
            return structure.IsBorderingTransparent(x, y, z, FaceDir.PosX, renderAtlas) || structure.IsBorderingTransparent(x, y, z, FaceDir.NegX, renderAtlas) ||
                   structure.IsBorderingTransparent(x, y, z, FaceDir.PosY, renderAtlas) || structure.IsBorderingTransparent(x, y, z, FaceDir.NegY, renderAtlas) ||
                   structure.IsBorderingTransparent(x, y, z, FaceDir.PosZ, renderAtlas) || structure.IsBorderingTransparent(x, y, z, FaceDir.NegZ, renderAtlas);
        }

        public static bool IsTransparent(this Structure structure, int x, int y, int z, RenderAtlas renderAtlas)
        {
            var block = structure[x, y, z];
            if (block == null || block.Id == "minecraft:air")
                return true;

            var data = renderAtlas[block.Id];
            if (data == null)
                return false;

            switch (data.Properties.Render)
            {
                case "none":
                case "transparent":
                case "cross":
                case "fence":
                case "skinny":
                case "cactus":
                case "layer":
                case "button":
                case "torch":
                case "pressureplate":
                case "door":
                case "wallsign":
                case "sign":
                case "stairs":
                case "slab":
                case "rail":
                case "ladder":
                case "wheat":
                    return true;
            }

            return false;
        }
    }
}
