using System.Collections.Generic;
using Newtonsoft.Json;

namespace Worldshape.Configuration
{
    public class BlockProperties
	{
		[JsonProperty("name", Required = Required.Always)]
		public string Name { get; set; }

		[JsonProperty("id", Required = Required.Always)]
		public int Id { get; set; }

		[JsonProperty("maxMeta", Required = Required.Always)]
		public int MaxMetadata { get; set; }

		[JsonProperty("render")]
		public string Render { get; set; }

		[JsonProperty("tint")]
		public bool UseBiomeTint { get; set; }

		[JsonProperty("texture", Required = Required.Always)]
		public List<string> Texture { get; set; }

        [JsonIgnore]
		internal string TextureDir { get; set; }
	}
}