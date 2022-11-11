﻿using System;
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

		public override Material OverrideMaterial => material ?? parent?.OverrideMaterial;

		private Shape[] subShapes;

		protected override void OnInit()
		{
			subShapes = new Shape[stepCount - 1];
			for (int i = 0; i < stepCount - 1; i++)
			{
				Cuboid step;
				float xn = 0f;
				float xp = 1f;
				float zn = 0f;
				float zp = 1f;
				float stepOffsetY = (i / (float)stepCount) * size.Y;
				float stepHeight = size.Y / stepCount;
				switch (stairsDirection)
				{
					case Direction.XNeg:
						xp = 1f - i / (stepCount - 1f);
						break;
					case Direction.XPos:
						xn = i / (stepCount - 1f);
						break;
					case Direction.ZNeg:
						zp = 1f - i / (stepCount - 1f);
						break;
					case Direction.ZPos:
						zn = i / (stepCount - 1f);
						break;
				}
				step = new Cuboid("step_" + i, new Vector3(size.X * xn, stepOffsetY, size.Z * zn), new Vector3(size.X * (xp - xn), stepHeight, size.Z * (zp - zn)));
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
