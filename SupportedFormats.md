## Key
	&: <Root tag type>
		<Key>: <Value type>=<Accepted values>!<Optional deprecation note>#<Optional comment>
			[]: <List entry type>
				<Required key>: <Required key type>
		?<Optional key>: <Value type>
		<Another key>: <Value type>
			$: <All children are of this type>

## Schematic structures (gzipped NBT, various mods)
	&: TAG_Compound
		Width: TAG_Short
		Height: TAG_Short
		Length: TAG_Short
		Materials: TAG_String=["Classic", "Pocket", "Alpha"]
		Blocks: TAG_Byte_Array#index = (Y * length + Z) * width + X
		?AddBlocks: TAG_Byte_Array#Each byte is two indices of Blocks, upper 4 bits is the upper 9-12 bits of the block at [i * 2], lower 4 is for the block at [i * 2 + 1]
		?Add: TAG_Byte_Array!Used previously by Schematica#Each byte is the 8-16 bits of the block at [i]
		?Data: TAG_Byte_Array#Each byte is the metadata of the block at [i]
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