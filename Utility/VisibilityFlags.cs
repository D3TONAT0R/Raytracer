using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
	[Flags]
	public enum VisibilityFlags
	{
		None = 0,
		Off = None,
		All = int.MaxValue,
		On = All,
		Direct = 1 << 0,
		Reflections = 1 << 1,
		Shadows = 1 << 2
	}
}
