## Key
### Binary Data
	<Struct Name>
		<Data type> <VariableName>
		<Data Type>{FlagsVariable&0b<Flag position>}
### NBT Data
	&: <Root tag type>
		<Key>: <Value type>=<Accepted values>!<Optional deprecation note>#<Optional comment>
			[]: <List entry type>
				<Required key>: <Required key type>
		?<Optional key>: <Value type>
		<Another key>: <Value type>
			$: <All children are of this type>

## SCARIF structures (brotli'd binary data, Parzi's Mods)
	TranslationMapEntry
		i16 Key # Numeric block ID
		s Value # Namespaced block name

	Block
		b BlockXZ # upper nibble is X, lower is Z
		b BlockY
		i16 BlockID
		b BlockFlags
		b{BlockFlags&0b1} BlockMetadata
		tagCompound{BlockFlags&0b10} BlockData

	ChunkDiff
		i32 ChunkX
		i32 ChunkZ
		i32 NumBlocks
		Block*NumBlocks DiffMap

	Root
		b Magic # "SCRF"
		i32 Version # 1
		i32 NumChunks # Number of chunks defined in the file
		i32 TranslationMapSize # Number of entries in the translation map
		TranslationMapEntry*TranslationMapSize BlockTranslationMap
		ChunkDiff*NumChunks ChunkDiffs

## Schematic structures (gzipped NBT, various mods)
	&: TAG_Compound
		Width: TAG_Short
		Height: TAG_Short
		Length: TAG_Short
		Materials: TAG_String=["Classic", "Pocket", "Alpha"]
		Blocks: TAG_Byte_Array#index = (Y * length + Z) * width + X
		?AddBlocks: TAG_Byte_Array#Each byte is two indices of Blocks, upper 4 bits is the upper 9-12 bits of the block at [i * 2], lower 4 is for the block at [i * 2 + 1]
		?Add: TAG_Byte_Array!Used previously by Schematica#Each byte is the 8-16 bits of the block at [i]
		?Metadata: TAG_Byte_Array#Each byte is the metadata of the block at [i]
		?Data: TAG_Byte_Array#Alternate for Metadata
		Entities: TAG_List
			[]: TAG_Compound#TODO: Standard entity format
		TileEntities: TAG_List
			[]: TAG_Compound#TODO: Standard tile entity format
		?Icon: TAG_Compound#TODO: Standard itemstack format, used by Schematica for icons
		?SchematicaMapping: TAG_Compound
			$: TAG_Short#All children are <namespaced block name>=<numeric block ID>
		?ExtendedMetdata: TAG_Compound#Anything else Schematica wants to pack inside the schematic
		?WEOriginX: TAG_Short#Origin of the structure in local space
		?WEOriginY: TAG_Short#Origin of the structure in local space
		?WEOriginZ: TAG_Short#Origin of the structure in local space
		?WEOriginX: TAG_Short#Origin of the structure in local space
		?WEOffsetX: TAG_Short#Offset of the structure in local space
		?WEOffsetY: TAG_Short#Offset of the structure in local space
		?WEOffsetZ: TAG_Short#Offset of the structure in local space
		?itemStackVersion: TAG_Byte=[17, 18]#MCEdit-2 only, 17 for numeric item IDs, 18 for namespaced strings
		?BlockIDs: TAG_Compound
			$: TAG_String#All children are <numeric block ID as string>=<namespaced block name>
		?ItemIDs: TAG_Compound#Only provided if itemStackversion is 17
			$: TAG_String#All children are <numeric block ID as string>=<namespaced block name>
		?TileTicks: TAG_List#MCEdit-Unified only, list of queued block updates
			[]: TAG_Compound
				i: TAG_String#Block ID
				t: TAG_Int#Ticks until processing occurs (negative means overdue)
				p: TAG_Int#Processing priority, lower is sooner
				x: TAG_Int
				y: TAG_Int
				z: TAG_Int
		?Biomes: TAG_Byte_Array#MCEdit-Unified only, a byte array containing all biomes in the schematic


## Structure Block structures (gzipped NBT, Minecraft)
	&: TAG_Compound
		DataVersion: TAG_Int
		author: TAG_String!Minecraft 1.3
		palette: TAG_List
			[]: TAG_Compound
				Name: TAG_String
				Properties: TAG_Compound
					$: TAG_String#All children are <property name>=<value>
		palettes: TAG_List
			[]: TAG_List
				[]: TAG_Compound
					Name: TAG_String
					Properties: TAG_Compound
						[]: TAG_String
		blocks: TAG_List
			[]: TAG_Compound
				state: TAG_Int
				pos: TAG_List
					[]: TAG_Int
				nbt: TAG_Compound
		entities: TAG_List
			[]: TAG_Compound
				pos: TAG_List
					[]: TAG_Double
				blockPos: TAG_List
					[]: TAG_Int
				nbt: TAG_Compound