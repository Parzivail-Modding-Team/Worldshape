using System;
using System.Drawing;

namespace Worldshape.Graphics.Primitive
{
	public class HsvColor
	{
		public static Color ToRgb(double h, double s, double v)
		{
			while (h < 0) { h += 360; };
			while (h >= 360) { h -= 360; };
			double r, g, b;
			if (v <= 0)
			{ r = g = b = 0; }
			else if (s <= 0)
			{
				r = g = b = v;
			}
			else
			{
				var hf = h / 60.0;
				var i = (int)Math.Floor(hf);
				var f = hf - i;
				var pv = v * (1 - s);
				var qv = v * (1 - s * f);
				var tv = v * (1 - s * (1 - f));
				switch (i)
				{

					// Red is the dominant color

					case 0:
						r = v;
						g = tv;
						b = pv;
						break;

					// Green is the dominant color

					case 1:
						r = qv;
						g = v;
						b = pv;
						break;
					case 2:
						r = pv;
						g = v;
						b = tv;
						break;

					// Blue is the dominant color

					case 3:
						r = pv;
						g = qv;
						b = v;
						break;
					case 4:
						r = tv;
						g = pv;
						b = v;
						break;

					// Red is the dominant color

					case 5:
						r = v;
						g = pv;
						b = qv;
						break;

					// Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

					case 6:
						r = v;
						g = tv;
						b = pv;
						break;
					case -1:
						r = v;
						g = pv;
						b = qv;
						break;

					// The color is not defined, we should throw an error.

					default:
						//LFATAL("i Value error in Pixel conversion, Value is %d", i);
						r = g = b = v; // Just pretend its black/white
						break;
				}
			}
			return Color.FromArgb(Clamp((int)(r * 255.0)), Clamp((int)(g * 255.0)), Clamp((int)(b * 255.0)));
		}

		/// <summary>
		/// Clamp a value to 0-255
		/// </summary>
		private static int Clamp(int i)
		{
			if (i < 0) return 0;
			return i > 255 ? 255 : i;
		}
	}
}