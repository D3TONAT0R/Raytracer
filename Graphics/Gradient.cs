using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
	public struct GradientKey
	{
		public float pos;
		public Color color;

		public GradientKey(float pos, Color color)
		{
			this.pos = pos;
			this.color = color;
		}
	}

	public class Gradient
	{

		public List<GradientKey> keys = new List<GradientKey>();

		public Gradient(params GradientKey[] keys)
		{
			this.keys = new List<GradientKey>(keys);
		}

		public Color Evaluate(float pos)
		{
			if(keys.Count == 0)
			{
				return Color.Magenta;
			}
			if(pos <= 0)
			{
				return keys[0].color;
			}
			else if(pos >= 1)
			{
				return keys[keys.Count - 1].color;
			}
			int b = 0;
			while(keys[b].pos < pos && b < keys.Count - 1)
			{
				b++;
			}
			int a = Math.Max(0, b - 1);
			float t = (pos - keys[a].pos) / (keys[b].pos - keys[a].pos);
			return Color.Lerp(keys[a].color, keys[b].color, t);
		}
	}
}
