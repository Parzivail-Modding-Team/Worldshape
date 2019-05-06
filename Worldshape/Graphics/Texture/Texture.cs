namespace Worldshape.Graphics.Texture
{
    public class Texture
    {
        public float MinU { get; }
        public float MinV { get; }
        public float MaxU { get; }
        public float MaxV { get; }

        public Texture(float minU, float minV, float maxU, float maxV)
        {
            MinU = minU;
            MinV = minV;
            MaxU = maxU;
            MaxV = maxV;
        }

        protected bool Equals(Texture other)
        {
            return MinU.Equals(other.MinU) && MinV.Equals(other.MinV) && MaxU.Equals(other.MaxU) && MaxV.Equals(other.MaxV);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Texture) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = MinU.GetHashCode();
                hashCode = (hashCode * 397) ^ MinV.GetHashCode();
                hashCode = (hashCode * 397) ^ MaxU.GetHashCode();
                hashCode = (hashCode * 397) ^ MaxV.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Texture left, Texture right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Texture left, Texture right)
        {
            return !Equals(left, right);
        }
    }
}