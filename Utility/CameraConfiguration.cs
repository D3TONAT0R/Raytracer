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

		[DataIdentifier("POSITION")]
		public Vector3 position;
		[DataIdentifier("ROTATION")]
		public Vector3 rotation;
		[DataIdentifier("FOV")]
		public float fieldOfView;
		[DataIdentifier("OFFSET")]
		public float offset;

		public CameraConfiguration(string name)
		{
			this.name = name;
		}
	}
}
