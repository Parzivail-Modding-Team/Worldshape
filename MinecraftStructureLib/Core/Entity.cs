using Substrate.Nbt;

namespace MinecraftStructureLib.Core
{
    public class Entity
    {
        public readonly TagNodeCompound Data;
        public readonly double X;
        public readonly double Y;
        public readonly double Z;

        public Entity(double x, double y, double z, TagNodeCompound data)
        {
            X = x;
            Y = y;
            Z = z;
            Data = data;
        }

        protected bool Equals(Entity other)
        {
            return Equals(Data, other.Data) && X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Entity) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Data != null ? Data.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Entity left, Entity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !Equals(left, right);
        }
    }
}