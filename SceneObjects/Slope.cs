using System;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{

	[ObjectIdentifier("SLOPE")]
	public class Slope : Prism
	{

		[DataIdentifier("DIRECTION")]
		public SlopeDirection direction;

		public Slope() : base() { }

		public Slope(string name, Vector3 position, Vector3 size, SlopeDirection dir) : base(name, position, size)
		{
			direction = dir;
		}

		protected override void OnInit()
		{
			base.OnInit();
			switch(direction)
			{
				case SlopeDirection.XNeg: cuts = new float[] { 0, 0, 1, 0 }; break;
				case SlopeDirection.XPos: cuts = new float[] { 1, 0, 0, 0 }; break;
				case SlopeDirection.ZNeg: cuts = new float[] { 0, 0, 0, 1 }; break;
				case SlopeDirection.ZPos: cuts = new float[] { 0, 1, 0, 0 }; break;
			}
		}
	}
}
