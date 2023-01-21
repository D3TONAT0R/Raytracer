using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.SceneObjects
{
	[ObjectIdentifier("WINDOW")]
	public class Window : SceneObject
	{
		[DataIdentifier("DIMS")]
		public Vector3 dimensions = Vector3.One;
		[DataIdentifier("AXIS")]
		public Axis axis;
		[DataIdentifier("BORDER")]
		public float border = 0.1f;
		[DataIdentifier("FILLDEPTH")]
		public float fillDepth;
		[DataIdentifier("MATERIAL")]
		public Material borderMaterial;
		[DataIdentifier("FILLMATERIAL")]
		public Material fillMaterial;

		private Cuboid[] borderCubes;
		private Cuboid fillCube;

		public float Width => dimensions.X;
		public float Height => dimensions.Y;
		public float Depth => dimensions.Z;
		/*
		public float Width => axis == Axis.X ? dimensions.Z : dimensions.X;
		public float Height => axis == Axis.Y ? dimensions.Z : dimensions.Y;
		public float Depth => axis == Axis.X ? dimensions.X : axis == Axis.Y ? dimensions.Y	 : dimensions.Z;
		*/

		public override bool CanContainShapes => true;

		public override Material OverrideMaterial => borderMaterial ?? parent.OverrideMaterial;

		protected override void OnInit()
		{
			float fill = fillDepth > 0 ? fillDepth : Depth * 0.25f;
			if(axis == Axis.X)
			{
				fillCube = new Cuboid("fill", new Vector3(-fill * 0.5f, border, border), new Vector3(fill, Height - border * 2, Width - border * 2));
				borderCubes = new Cuboid[]
				{
					new Cuboid("b0", new Vector3(-Depth * 0.5f, 0, 0), new Vector3(Depth, border, Width)),
					new Cuboid("b1", new Vector3(-Depth * 0.5f, Height - border, 0), new Vector3(Depth, border, Width)),
					new Cuboid("b2", new Vector3(-Depth * 0.5f, border, 0), new Vector3(Depth, Height - border * 2, border)),
					new Cuboid("b3", new Vector3(-Depth * 0.5f, border, Width - border), new Vector3(Depth, Height - border * 2, border))
				};
			}
			else if(axis == Axis.Y)
			{
				fillCube = new Cuboid("fill", new Vector3(border, -fill * 0.5f, border), new Vector3(Width - border * 2, fill, Height - border * 2));
				borderCubes = new Cuboid[]
				{
					new Cuboid("b0", new Vector3(0, -Depth * 0.5f, 0), new Vector3(Width, Depth, border)),
					new Cuboid("b1", new Vector3(0, -Depth * 0.5f, Height - border), new Vector3(Width, Depth, border)),
					new Cuboid("b2", new Vector3(0, -Depth * 0.5f, border), new Vector3(border, Depth, Height - border * 2)),
					new Cuboid("b3", new Vector3(Width - border, -Depth * 0.5f, border), new Vector3(border, Depth, Height - border * 2))
				};
			}
			else
			{
				fillCube = new Cuboid("fill", new Vector3(border, border, -fill * 0.5f), new Vector3(Width - border * 2, Height - border * 2, fill));
				borderCubes = new Cuboid[]
				{
					new Cuboid("b0", new Vector3(0, 0, -Depth * 0.5f), new Vector3(Width, border, Depth)),
					new Cuboid("b1", new Vector3(0, Height - border, -Depth * 0.5f), new Vector3(Width, border, Depth)),
					new Cuboid("b2", new Vector3(0, border, -Depth * 0.5f), new Vector3(border, Height - border * 2, Depth)),
					new Cuboid("b3", new Vector3(Width - border, border, -Depth * 0.5f), new Vector3(border, Height - border * 2, Depth))
				};
			}

			fillCube.parent = this;
			fillCube.material = fillMaterial;
			foreach(var c in borderCubes)
			{
				c.parent = this;
			}

			foreach(var c in GetSubShapes())
			{
				c.Initialize();
			}
		}

		public override void SetupForRendering()
		{
			foreach(var c in GetSubShapes())
			{
				c.SetupForRendering();
			}
		}

		public override IEnumerable<T> GetContainedObjectsOfType<T>()
		{
			if(typeof(Shape).IsAssignableFrom(typeof(T)))
			{
				foreach(var s in GetSubShapes())
				{
					yield return (T)(SceneObject)s;
				}
			}
		}

		public override IEnumerable<Shape> GetIntersectingShapes(Ray ray)
		{
			foreach(var c in GetSubShapes())
			{
				foreach(var s in c.GetIntersectingShapes(ray)) yield return s;
			}
		}

		public IEnumerable<Cuboid> GetSubShapes()
		{
			yield return fillCube;
			for(int i = 0; i < borderCubes.Length; i++)
			{
				yield return borderCubes[i];
			}
		}

		public override AABB GetTotalShapeAABB()
		{
			var aabb = fillCube.ShapeAABB;
			for(int i = 0; i < borderCubes.Length; i++)
			{
				aabb = aabb.Join(borderCubes[i].ShapeAABB);
			}
			return aabb;
		}
	}
}
