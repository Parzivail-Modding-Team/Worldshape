using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Brotli;
using MinecraftStructureLib.Core;
using MinecraftStructureLib.Core.Translation;
using Substrate.Nbt;

namespace MinecraftStructureLib.Loader.Scarif
{
    public class ScarifLoader : IStructureLoader
    {
        private static void EnforceBounds(int x, byte y, int z, ref BlockPos lowestPos, ref BlockPos highestPos)
        {
            if (x < lowestPos.X)
                lowestPos = new BlockPos(x, lowestPos.Y, lowestPos.Z);
            if (y < lowestPos.Y)
                lowestPos = new BlockPos(lowestPos.X, y, lowestPos.Z);
            if (z < lowestPos.Z)
                lowestPos = new BlockPos(lowestPos.X, lowestPos.Y, z);
            if (x > highestPos.X)
                highestPos = new BlockPos(x, highestPos.Y, highestPos.Z);
            if (y > highestPos.Y)
                highestPos = new BlockPos(highestPos.X, y, highestPos.Z);
            if (z > highestPos.Z)
                highestPos = new BlockPos(highestPos.X, highestPos.Y, z);
        }

        private static string ReadNullTerminatedString(BinaryReader s)
        {
            var str = new StringBuilder();
            while (true)
            {
                var b = s.ReadByte();
                if (b == 0)
                    return str.ToString();
                str.Append((char) b);
            }
        }

        /// <inheritdoc />
        public bool CanLoad(string filename)
        {
            return Path.GetExtension(filename) == ".scrf";
        }

        /// <inheritdoc />
        public Structure Load(string filename)
        {
            using (var fs = File.OpenRead(filename))
            using (var bs = new BrotliStream(fs, CompressionMode.Decompress))
            using (var s = new BinaryReader(bs))
            {
                var idMap = new TranslationMap();
                var diffMap = new DiffMap();

                var identBytes = new byte[ScarifStructure.Magic.Length];
                var read = s.Read(identBytes, 0, identBytes.Length);
                var ident = Encoding.UTF8.GetString(identBytes);
                if (ident != ScarifStructure.Magic || read != identBytes.Length)
                    throw new IOException("Input file not SCARIF structure");

                var version = s.ReadInt32();
                if (version != 1)
                    throw new IOException("Input file not SCARIF v1");
                var numChunks = s.ReadInt32();
                var numIdMapEntries = s.ReadInt32();

                for (var entryIdx = 0; entryIdx < numIdMapEntries; entryIdx++)
                {
                    var id = s.ReadInt16();
                    var name = ReadNullTerminatedString(s);
                    idMap.Add(id, name);
                }

                var lowestPos = new BlockPos(int.MaxValue, int.MaxValue, int.MaxValue);
                var highestPos = new BlockPos(int.MinValue, int.MinValue, int.MinValue);

                for (var chunkIdx = 0; chunkIdx < numChunks; chunkIdx++)
                {
                    var chunkX = s.ReadInt32();
                    var chunkZ = s.ReadInt32();
                    var numBlocks = s.ReadInt32();

                    var blocks = new Dictionary<BlockPos, Block>();

                    for (var blockIdx = 0; blockIdx < numBlocks; blockIdx++)
                    {
                        // Format:
                        // 0b 0000 1111
                        //    xxxx zzzz
                        var xz = s.ReadByte();

                        var x = (byte) ((xz & 0xF0) >> 4);
                        var z = (byte) (xz & 0x0F);
                        var y = s.ReadByte();

                        EnforceBounds(chunkX * 16 + x, y, chunkZ * 16 + z, ref lowestPos, ref highestPos);

                        var id = s.ReadInt16();
                        var flags = (BlockFlags) s.ReadByte();

                        byte metadata = 0;
                        NbtTree tileTag = null;

                        if (flags.Has(BlockFlags.Metadata))
                            metadata = s.ReadByte();
                        if (flags.Has(BlockFlags.Nbt))
                        {
                            var len = s.ReadInt32();
                            if (len <= 0)
                                throw new IOException("Zero-length NBT present");
                            var bytes = s.ReadBytes(len);
                            using (var ms = new MemoryStream(bytes))
                            {
                                tileTag = new NbtTree(ms);
                            }
                        }

                        if (idMap.ContainsKey(id))
                            blocks[new BlockPos(x, y, z)] = new Block(idMap.TranslateBlock(id), metadata, tileTag?.Root);
                        else
                            throw new IOException($"Unknown block ID found: {id}");
                    }

                    diffMap.Add(new ChunkPosition(chunkX, chunkZ), blocks);
                }

                return new ScarifStructure(idMap, diffMap, lowestPos, highestPos);
            }
        }
    }
}