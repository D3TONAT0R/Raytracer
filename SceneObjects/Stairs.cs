using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
	[ObjectIdentifier("STAIRS")]
	public class Stairs : SceneObject
	{
		public enum Direction { XNeg, XPos, ZNeg, ZPos }

		[DataIdentifier("SIZE")]
		public Vector3 size;
		[DataIdentifier("DIRECTION")]
		public Direction stairsDirection;
		[DataIdentifier("STEPS")]
		public int stepCount;
		[DataIdentifier("MATERIAL")]
		public Material material;

		public override Material OverrideMaterial => material ?? parent.OverrideMaterial;

		private Shape[] subShapes;

		protected override void OnInit()
		{
			subShapes = new Shape[stepCount - 1];
			for (int i = 0; i < stepCount - 1; i++)
			{
				var stepOffsetX = (i / (stepCount - 1f)) * size.X;
				var stepOffsetY = (i / (float)stepCount) * size.Y;
				var stepHeight = size.Y / stepCount;
				var step = new Cuboid("step_" + i, new Vector3(stepOffsetX, stepOffsetY, 0), new Vector3(size.X - stepOffsetX, stepHeight, size.Z));
				step.parent = this;
				subShapes[i] = step;
			}
		}

		public override IEnumerable<T> GetContainedObjectsOfType<T>()
		{
			foreach(var s in subShapes)
			{
				foreach (var s1 in s.GetContainedObjectsOfType<T>())
				{
					yield return s1;
				}
			}
		}
	}
}
