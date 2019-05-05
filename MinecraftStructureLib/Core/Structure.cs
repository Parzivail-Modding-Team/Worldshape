using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace MinecraftStructureLib.Core
{
    public abstract class Structure
    {
        public int Length { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        /// <summary>
        ///     Returns the block at the given position relative to the origin of the file
        /// </summary>
        /// <param name="x">The X coordinate of the block</param>
        /// <param name="y">The Y coordinate of the block</param>
        /// <param name="z">The Z coordinate of the block</param>
        /// <returns>The block at the specified coordinates, or null if one is not explicitly defined</returns>
        public abstract Block this[int x, int y, int z] { get; set; }

        /// <summary>
        ///     Saves the structure to the given file
        /// </summary>
        /// <param name="filename">The file to save to</param>
        public abstract void Save(string filename);

        public abstract string GetFileExtension();

        public bool Contains(int x, int y, int z)
        {
            return x >= 0 && y >= 0 && z >= 0 && x < Width && y < Height && z < Length;
        }

        public bool IsBorderingAir(int x, int y, int z, FaceDir face)
        {
            switch (face.Facing)
            {
                case FaceDir.Dir.PosX:
                    if (x == Width - 1)
                        return true;
                    break;
                case FaceDir.Dir.NegX:
                    if (x == 0)
                        return true;
                    break;
                case FaceDir.Dir.PosY:
                    if (y == Height - 1)
                        return true;
                    break;
                case FaceDir.Dir.NegY:
                    if (y == 0)
                        return true;
                    break;
                case FaceDir.Dir.PosZ:
                    if (z == Length - 1)
                        return true;
                    break;
                case FaceDir.Dir.NegZ:
                    if (z == 0)
                        return true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return IsAir(x + face.X, y + face.Y, z + face.Z);
        }

        public bool IsBorderingAir(int x, int y, int z)
        {
            return IsBorderingAir(x, y, z, FaceDir.PosX) || IsBorderingAir(x, y, z, FaceDir.NegX) ||
                   IsBorderingAir(x, y, z, FaceDir.PosY) || IsBorderingAir(x, y, z, FaceDir.NegY) ||
                   IsBorderingAir(x, y, z, FaceDir.PosZ) || IsBorderingAir(x, y, z, FaceDir.NegZ);
        }

        public bool IsAir(int x, int y, int z)
        {
            var block = this[x, y, z];
            return block == null || block.Id == "minecraft:air";
        }
    }

    public class FaceDir
    {
        public static readonly FaceDir PosX = new FaceDir(Dir.PosX, 1, 0, 0);
        public static readonly FaceDir NegX = new FaceDir(Dir.NegX, -1, 0, 0);
        public static readonly FaceDir PosY = new FaceDir(Dir.PosY, 0, 1, 0);
        public static readonly FaceDir NegY = new FaceDir(Dir.NegY, 0, -1, 0);
        public static readonly FaceDir PosZ = new FaceDir(Dir.PosZ, 0, 0, 1);
        public static readonly FaceDir NegZ = new FaceDir(Dir.NegZ, 0, 0, -1);

        public Dir Facing { get; }
        public int X { get; }
        public int Y { get; }
        public int Z { get; }

        private FaceDir(Dir dir, int x, int y, int z)
        {
            Facing = dir;
            X = x;
            Y = y;
            Z = z;
        }

        public enum Dir
        {
            PosX,
            NegX,
            PosY,
            NegY,
            PosZ,
            NegZ
        }
    }
}