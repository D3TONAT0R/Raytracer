using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
	[ObjectIdentifier("CAMERA_CONFIGURATION")]
	public class CameraConfiguration
	{
		public string name;

		[DataIdentifier("POSITION", 0.1f)]
		public Vector3 position;
		[DataIdentifier("ROTATION", 1f)]
		public Vector3 rotation;
		[DataIdentifier("FOV", 1f)]
		public float fieldOfView;
		[DataIdentifier("OFFSET", 0.1f)]
		public float offset;

		public CameraConfiguration(string name)
		{
			this.name = name;
		}
	}
}
