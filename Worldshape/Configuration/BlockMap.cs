using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Worldshape.Configuration
{
    public class BlockMap
	{
		[JsonProperty("minecraft", Required = Required.Always)]
		public string MinecraftVersion { get; set; }

		[JsonProperty("textureDir", Required = Required.Always)]
		public string RootTextureDir { get; set; }

		[JsonProperty("entries", Required = Required.Always)]
		public List<BlockProperties> Entries { get; set; }
	}
}
