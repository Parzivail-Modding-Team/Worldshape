using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Brotli;
using MinecraftStructureLib.Core;
using Substrate.Nbt;

namespace MinecraftStructureLib.Scarif
{
    public class ScarifStructure : Structure
    {
        private const string Magic = "SCRF";

        public TranslationMap TranslationMap;
        public DiffMap DiffMap;
        private BlockPos _lowestPosition;
        private BlockPos _highestPosition;

        public ScarifStructure(TranslationMap translationMap)
        {
            DiffMap = new DiffMap();
            TranslationMap = translationMap;
        }

        private ScarifStructure(TranslationMap translationMap, DiffMap diffMap) : this(translationMap)
        {
            DiffMap = diffMap;
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
                    f.Write((byte)0);
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
                        var x = (byte)(block.Key.X - pair.Key.X * 16) & 0x0F;
                        var z = (byte)(block.Key.Z - pair.Key.Z * 16) & 0x0F;
                        f.Write((byte)((x << 4) | z));
                        f.Write((byte)block.Key.Y);
                        f.Write((short)TranslateBlock(block.Value.Id));

                        var flags = block.Value.CreateFlags();
                        f.Write((byte)flags);

                        if (flags.HasFlag(BlockFlags.Metadata))
                            f.Write((byte)block.Value.Metadata);

                        if (!flags.HasFlag(BlockFlags.Nbt)) continue;

                        using (var memstream = new MemoryStream())
                        {
                            // Terrible hack to make the NBT in the format that MC likes
                            block.Value.Data.WriteTo(memstream);
                            memstream.Seek(0, SeekOrigin.Begin);
                            var len = memstream.Length;
                            f.Write((int)len);
                            var b = new byte[(int)len];
                            memstream.Read(b, 0, (int)len);
                            f.Write(b);
                        }
                    }
                }
            }
        }

        private short TranslateBlock(string id)
        {
            foreach (var pair in TranslationMap)
                if (pair.Value == id)
                    return pair.Key;
            throw new IOException($"Unknown block ID found: {id}");
        }

        private string TranslateBlock(short id)
        {
            if (TranslationMap.TryGetValue(id, out var namespacedId))
                return namespacedId;
            throw new IOException($"Unknown block ID found: {id}");
        }

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
            set => throw new System.NotImplementedException();
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

        public override void Load(string filename)
        {
            using (var fs = File.OpenRead(filename))
            using (var bs = new BrotliStream(fs, CompressionMode.Decompress))
            using (var s = new BinaryReader(bs))
            {
                var idMap = new TranslationMap();
                var diffMap = new DiffMap();

                var identBytes = new byte[Magic.Length];
                var read = s.Read(identBytes, 0, identBytes.Length);
                var ident = Encoding.UTF8.GetString(identBytes);
                if (ident != Magic || read != identBytes.Length)
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
                        // 0x 0000 1111
                        //    xxxx zzzz
                        var xz = s.ReadByte();

                        var x = (byte)((xz & 0xF0) >> 4);
                        var z = (byte)(xz & 0x0F);
                        var y = s.ReadByte();

                        EnforceBounds(chunkX * 16 + x, y, chunkZ * 16 + z, ref lowestPos, ref highestPos);

                        var id = s.ReadInt16();
                        var flags = (BlockFlags)s.ReadByte();

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
                                tileTag = new NbtTree(ms);
                        }

                        if (idMap.ContainsKey(id))
                            blocks[new BlockPos(x, y, z)] = new Block(TranslateBlock(id), metadata, tileTag);
                        else
                            throw new IOException($"Unknown block ID found: {id}");
                    }

                    diffMap.Add(new ChunkPosition(chunkX, chunkZ), blocks);
                }

                TranslationMap = idMap;
                DiffMap = diffMap;
                _lowestPosition = lowestPos;
                _highestPosition = highestPos;
            }
        }

        private static void EnforceBounds(int x, byte y, int z, ref BlockPos lowestPos, ref BlockPos highestPos)
        {
            if (x < lowestPos.X)
                lowestPos = new BlockPos(lowestPos.X, lowestPos.Y, lowestPos.Z);
            if (y < lowestPos.X)
                lowestPos = new BlockPos(lowestPos.X, lowestPos.Y, lowestPos.Z);
            if (z < lowestPos.X)
                lowestPos = new BlockPos(lowestPos.X, lowestPos.Y, lowestPos.Z);
            if (x > highestPos.X)
                highestPos = new BlockPos(highestPos.X, highestPos.Y, highestPos.Z);
            if (y > lowestPos.X)
                highestPos = new BlockPos(highestPos.X, highestPos.Y, highestPos.Z);
            if (z > lowestPos.X)
                highestPos = new BlockPos(highestPos.X, highestPos.Y, highestPos.Z);
        }

        private static string ReadNullTerminatedString(BinaryReader s)
        {
            var str = new StringBuilder();
            while (true)
            {
                var b = s.ReadByte();
                if (b == 0)
                    return str.ToString();
                str.Append((char)b);
            }
        }
    }
}