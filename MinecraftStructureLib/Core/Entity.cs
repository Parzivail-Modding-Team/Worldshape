using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Substrate.Nbt;

namespace MinecraftStructureLib.Core
{
	public class Entity
	{
        public readonly double X;
        public readonly double Y;
        public readonly double Z;
        public readonly TagNodeCompound Data;

        public Entity(double x, double y, double z, TagNodeCompound data)
        {
            X = x;
            Y = y;
            Z = z;
            Data = data;
        }

        public bool Equals(Entity other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && Equals(Data, other.Data);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            return obj is Entity other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                hashCode = (hashCode * 397) ^ (Data != null ? Data.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(Entity left, Entity right)
        {
            return left != null && left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return left != null && !left.Equals(right);
        }
    }
}
