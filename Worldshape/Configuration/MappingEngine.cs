using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Worldshape.Configuration
{
    public class MappingEngine
    {
        public List<BlockProperties> Mappings { get; }

        public MappingEngine()
        {
            Mappings = new List<BlockProperties>();

            foreach (var file in Directory.GetFiles("Resources/mappings/"))
            {
                var mapping = JsonConvert.DeserializeObject<BlockMap>(File.ReadAllText(file));
                foreach (var entry in mapping.Entries) entry.TextureDir = mapping.RootTextureDir;
                Mappings.AddRange(mapping.Entries);
            }
        }
        
        public BlockProperties this[string name] => GetMapping(name);

        private BlockProperties GetMapping(string name)
        {
            return Mappings.FirstOrDefault(properties => properties.Name == name);
        }
    }
}
