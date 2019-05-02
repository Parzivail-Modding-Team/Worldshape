using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Brotli;
using MinecraftStructureLib.Core;

namespace MinecraftStructureLib.Loader.Scarif
{
    public class ScarifStructure : Structure
    {
        public const string Magic = "SCRF";
        public const string FileExtension = ".scrf";
        private readonly BlockPos _lowestPosition;
        public readonly DiffMap DiffMap;

        public readonly TranslationMap TranslationMap;

        /// <inheritdoc />
        public override Block this[int x, int y, int z]
        {
            get
            {
                x += _lowestPosition.X;
                y += _lowestPosition.Y;
                z += _lowestPosition.Z;

                return GetWorldBlock(x, y, z);
            }
            set => throw new NotImplementedException();
        }

        public ScarifStructure(TranslationMap translationMap, DiffMap diffMap, BlockPos lowestPosition,
            BlockPos highestPosition)
        {
            TranslationMap = translationMap;
            DiffMap = diffMap;
            _lowestPosition = lowestPosition;
            Width = highestPosition.X - lowestPosition.X;
            Height = highestPosition.Y - lowestPosition.Y;
            Length = highestPosition.Z - lowestPosition.Z;
        }

        private void TrimMappings()
        {
            var keysToRemove = new List<short>();
            foreach (var pair in TranslationMap)
            {
                if (HasAnyBlocksWithId(pair.Value))
                    continue;

                keysToRemove.Add(pair.Key);
            }

            foreach (var key in keysToRemove) TranslationMap.Remove(key);
        }

        /// <inheritdoc />
        public override string GetFileExtension()
        {
            return FileExtension;
        }

        private bool HasAnyBlocksWithId(string id)
        {
            return DiffMap.Any(chunk => chunk.Value.Any(entry => id == entry.Value.Id));
        }

        public override void Save(string filename)
        {
            using (var fs = File.OpenWrite(filename))
            using (var bs = new BrotliStream(fs, CompressionMode.Compress))
            using (var f = new BinaryWriter(bs))
            {
                var ident = Magic.ToCharArray();

                f.Write(ident);
                f.Write(1); // Version
                f.Write(DiffMap.Keys.Count); // Keys = Chunks

                TrimMappings();

                f.Write(TranslationMap.Keys.Count);

                foreach (var pair in TranslationMap)
                {
                    f.Write(pair.Key);

                    var buffer = Encoding.UTF8.GetBytes(pair.Value);
                    f.Write(buffer);
                    f.Write((byte) 0);
                }

                // For each chunk
                foreach (var pair in DiffMap)
                {
                    // Write out the chunk pos and how many blocks it has
                    f.Write(pair.Key.X);
                    f.Write(pair.Key.Z);
                    f.Write(pair.Value.Count);

                    // Write out each block's position and data
                    foreach (var block in pair.Value)
                    {
                        var x = (byte) (block.Key.X - pair.Key.X * 16) & 0x0F;
                        var z = (byte) (block.Key.Z - pair.Key.Z * 16) & 0x0F;
                        f.Write((byte) ((x << 4) | z));
                        f.Write((byte) block.Key.Y);
                        f.Write(TranslationMap.TranslateBlock(block.Value.Id));

                        var flags = block.Value.CreateFlags();
                        f.Write((byte) flags);

                        if (flags.HasFlag(BlockFlags.Metadata))
                            f.Write((byte) block.Value.Metadata);

                        if (!flags.HasFlag(BlockFlags.Nbt)) continue;

                        using (var memstream = new MemoryStream())
                        {
                            // Terrible hack to make the NBT in the format that MC likes
                            block.Value.Data.WriteTo(memstream);
                            memstream.Seek(0, SeekOrigin.Begin);
                            var len = memstream.Length;
                            f.Write((int) len);
                            var b = new byte[(int) len];
                            memstream.Read(b, 0, (int) len);
                            f.Write(b);
                        }
                    }
                }
            }
        }

        public Block GetWorldBlock(int x, int y, int z)
        {
            var chunkX = x / 16;
            var chunkZ = z / 16;
            var chunkPos = new ChunkPosition(chunkX, chunkZ);
            if (!DiffMap.TryGetValue(chunkPos, out var blocks))
                return null;

            var blockPos = new BlockPos(x % 16, y, z % 16);
            return !blocks.TryGetValue(blockPos, out var block) ? null : block;
        }
    }
}