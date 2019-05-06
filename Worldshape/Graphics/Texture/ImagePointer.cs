﻿using System.Drawing;

namespace Worldshape.Graphics.Texture
{
    internal class ImagePointer
    {
        public string TextureName { get; }
        public string TexturePath { get; }
        public Size Size { get; }
        public Point Position { get; set; }

        public ImagePointer(string textureName, string texturePath, Size size)
        {
            TextureName = textureName;
            TexturePath = texturePath;
            Size = size;
            Position = default;
        }

        protected bool Equals(ImagePointer other)
        {
            return string.Equals(TextureName, other.TextureName) && string.Equals(TexturePath, other.TexturePath) && Size.Equals(other.Size);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ImagePointer) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (TextureName != null ? TextureName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TexturePath != null ? TexturePath.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Size.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(ImagePointer left, ImagePointer right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ImagePointer left, ImagePointer right)
        {
            return !Equals(left, right);
        }
    }
}