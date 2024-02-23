namespace Raytracer
{
	public struct ByteColor
	{
		public byte r;
		public byte g;
		public byte b;
		public byte a;

		public ByteColor(byte r, byte g, byte b, byte a)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		public ByteColor(byte r, byte g, byte b) : this(r, g, b, 255)
		{

		}
	}
}
