using System.Drawing;

namespace Worldshape.Layout
{
    internal class Node
    {
        public Node Left;
        public Node Right;
        public Rectangle Rectangle;
        public bool Filled;

        public Node Insert(Rectangle rect)
        {
            if (Left != null)
                return Left.Insert(rect) ?? Right.Insert(rect);

            if (Filled || !FitsIn(this.Rectangle, rect))
                return null;

            if (AreSameSize(this.Rectangle, rect))
            {
                Filled = true;
                return this;
            }

            Left = new Node();
            Right = new Node();
            var widthDiff = this.Rectangle.Width - rect.Width;
            var heightDiff = this.Rectangle.Height - rect.Height;

            if (widthDiff > heightDiff)
            {
                // split literally into left and right, putting the rect on the left.
                Left.Rectangle = new Rectangle(this.Rectangle.X, this.Rectangle.Y, rect.Width, this.Rectangle.Height);
                Right.Rectangle = new Rectangle(this.Rectangle.X + rect.Width, this.Rectangle.Y, this.Rectangle.Width - rect.Width, this.Rectangle.Height);
            }
            else
            {
                // split into top and bottom, putting rect on top.
                Left.Rectangle = new Rectangle(this.Rectangle.X, this.Rectangle.Y, this.Rectangle.Width, rect.Height);
                Right.Rectangle = new Rectangle(this.Rectangle.X, this.Rectangle.Y + rect.Height, this.Rectangle.Width, this.Rectangle.Height - rect.Height);
            }
            return Left.Insert(rect);
        }

        private static bool AreSameSize(Rectangle a, Rectangle b) => a.Width == b.Width && a.Height == b.Height;

        private static bool FitsIn(Rectangle host, Rectangle query) => host.Width >= query.Width && host.Height >= query.Height;
    }
}
