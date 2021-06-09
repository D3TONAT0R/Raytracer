using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {
	public struct Color {

		public float r;
		public float g;
		public float b;
		public float a;

		public static readonly Color Black = new Color(0, 0, 0, 1);
		public static readonly Color Gray = new Color(0.5f, 0.5f, 0.5f, 1);
		public static readonly Color White = new Color(1, 1, 1, 1);
		public static readonly Color Magenta = new Color(1, 0, 1, 1);
		public static readonly Color Red = new Color(1, 0, 0, 1);
		public static readonly Color Green = new Color(0, 1, 0, 1);
		public static readonly Color Blue = new Color(0, 0, 1, 1);

		public System.Drawing.Color DrawingColor {
			get {
				var cr = ToColorByte(r);
				var cg = ToColorByte(g);
				var cb = ToColorByte(b);
				var ca = ToColorByte(a);
				return System.Drawing.Color.FromArgb(ca, cr, cg, cb);
			}
		}

		public Color(float r, float g, float b, float a) {
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		public Color(float r, float g, float b) : this(r, g, b, 1) {

		}

		public Color SetAlpha(float alpha) {
			return SetComponent(3, alpha);
		}

		public Color SetComponent(int i, float v) {
			Color c = new Color(r, g, b, a);
			if(i == 0) {
				c.r = v;
			} else if(i == 1) {
				c.g = v;
			} else if(i == 2) {
				c.b = v;
			} else if(i == 3) {
				c.a = v;
			} else {
				throw new IndexOutOfRangeException();
			}
			return c;
		}

		public static implicit operator Color(System.Drawing.Color dc) {
			return new Color(dc.R / 255f, dc.G / 255f, dc.B / 255f, dc.A / 255f);
		}

		byte ToColorByte(float f) {
			int i = (int)(f * 255);
			return (byte)Math.Min(255, Math.Max(0, i));
		}

		public static Color operator +(Color l, Color r) {
			return new Color(l.r + r.r, l.g + r.g, l.b + r.b, l.a + r.a);
		}

		public static Color operator *(Color l, Color r) {
			return new Color(l.r * r.r, l.g * r.g, l.b * r.b, l.a * r.a);
		}

		public static Color operator *(Color l, float r) {
			return new Color(l.r * r, l.g * r, l.b * r, l.a * r);
		}

		public static Color Lerp(Color a, Color b, float t) {
			return new Color(
				Lerp(a.r, b.r, t),
				Lerp(a.g, b.g, t),
				Lerp(a.b, b.b, t),
				Lerp(a.a, b.a, t)
			);
		}

		private static float Lerp(float a, float b, float t) {
			return a + (b - a) * t;
		}
	}
}
