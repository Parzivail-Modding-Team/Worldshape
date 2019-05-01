using Substrate.Nbt;

namespace MinecraftStructureLib.Core
{
	public class Block
	{
		public readonly string Id;
		public readonly int Metadata;
		public readonly NbtTree Data;

		public Block(string id, int metadata = 0, NbtTree data = null)
		{
			Id = id;
			Metadata = metadata;
			Data = data;
		}

		public bool Equals(Block other)
		{
			return string.Equals(Id, other.Id) && Metadata == other.Metadata;
		}

		public override bool Equals(object obj)
		{
			if (obj is null) return false;
			return obj is Block other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Id != null ? Id.GetHashCode() : 0) * 397) ^ Metadata;
			}
		}

		public static bool operator ==(Block left, Block right)
		{
			return left != null && left.Equals(right);
		}

		public static bool operator !=(Block left, Block right)
		{
			return left != null && !left.Equals(right);
		}
	}
}