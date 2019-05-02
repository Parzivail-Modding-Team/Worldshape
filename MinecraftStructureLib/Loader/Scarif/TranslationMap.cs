using System.Collections.Generic;
using System.IO;

namespace MinecraftStructureLib.Loader.Scarif
{
    public class TranslationMap : Dictionary<short, string>
    {
        public short TranslateBlock(string id)
        {
            foreach (var pair in this)
                if (pair.Value == id)
                    return pair.Key;
            throw new IOException($"Unknown block ID found: {id}");
        }

        public string TranslateBlock(short id)
        {
            if (TryGetValue(id, out var namespacedId))
                return namespacedId;
            throw new IOException($"Unknown block ID found: {id}");
        }
    }
}