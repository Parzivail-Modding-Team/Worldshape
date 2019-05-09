﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Nett;

namespace Worldshape.Configuration
{
	class Config
	{
		private const string ConfigFile = "config.toml";

		[TomlMember(Key = "saveAtlas"),
		 TomlComment("Whether or not to save a copy of the texture atlas for debugging purposes")]
		public bool SaveAtlas { get; set; } = false;

		[TomlMember(Key = "unpackTextures"),
		 TomlComment("Whether or not to unpack the provided assets")]
		public bool UnpackTextures { get; set; } = true;

		public static Config Load()
		{
			if (!File.Exists(ConfigFile))
				new Config().Save();
			return Toml.ReadFile<Config>(ConfigFile);
		}

		public void Save()
		{
			Toml.WriteFile(this, ConfigFile);
		}
	}
}
