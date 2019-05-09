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
        public static bool IsBorderingTransparent(this Structure structure, int x, int y, int z, FaceDir face, BlockAtlas blockAtlas)
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

            return structure.IsTransparent(x + face.X, y + face.Y, z + face.Z, blockAtlas);
        }

        public static bool IsBorderingTransparent(this Structure structure, int x, int y, int z, BlockAtlas blockAtlas)
        {
            return structure.IsBorderingTransparent(x, y, z, FaceDir.PosX, blockAtlas) || structure.IsBorderingTransparent(x, y, z, FaceDir.NegX, blockAtlas) ||
                   structure.IsBorderingTransparent(x, y, z, FaceDir.PosY, blockAtlas) || structure.IsBorderingTransparent(x, y, z, FaceDir.NegY, blockAtlas) ||
                   structure.IsBorderingTransparent(x, y, z, FaceDir.PosZ, blockAtlas) || structure.IsBorderingTransparent(x, y, z, FaceDir.NegZ, blockAtlas);
        }

        public static bool IsTransparent(this Structure structure, int x, int y, int z, BlockAtlas blockAtlas)
        {
            var block = structure[x, y, z];
            if (block == null || block.Id == "minecraft:air")
                return true;

            var data = blockAtlas[block.Id];
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
