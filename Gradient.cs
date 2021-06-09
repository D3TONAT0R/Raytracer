using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {
	public class Gradient {

		//SortedDictionary<float, Color> keys;
		List<float> positions;
		List<Color> colors;

		public Gradient(params (float p, Color c)[] keys) {
			//this.keys = new SortedDictionary<float, Color>();
			positions = new List<float>();
			colors = new List<Color>();
			foreach(var k in keys) {
				positions.Add(k.p);
				colors.Add(k.c);
			}
		}

		public Color Evaluate(float pos) {
			if(pos <= 0) {
				return colors[0];
			} else if(pos >= 1) {
				return colors[colors.Count - 1];
			}
			int b = 0;
			while(positions[b] < pos && b < positions.Count - 1) {
				b++;
			}
			int a = Math.Max(0, b - 1);
			float t = (pos - positions[a]) / (positions[b] - positions[a]);
			return LerpColor(colors[a], colors[b], t);
		}

		private Color LerpColor(Color a, Color b, float t) {
			if(t <= 0) {
				return a;
			} else if(t >= 1) {
				return b;
			}
			var cr = Lerp(a.r, b.r, t);
			var cg = Lerp(a.g, b.g, t);
			var cb = Lerp(a.b, b.b, t);
			var ca = Lerp(a.a, b.a, t);
			return new Color(cr, cg, cb, ca);
		}

		private float Lerp(float a, float b, float t) {
			return a + (b - a) * t;
		}
	}
}
