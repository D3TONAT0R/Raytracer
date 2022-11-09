/*
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Device = SharpDX.Direct3D11.Device;

namespace ConsoleGameEngine.source.Raytracer.GPUTest
{
	public class GPUTest
	{
		public GPUTest()
		{
			var device = new Device(DriverType.Hardware, DeviceCreationFlags.None);
			var context = device.ImmediateContext;

			var bytecode = ShaderBytecode.CompileFromFile("test.hlsl", "testfunction", "vs_4_0");
			var shader = new PixelShader(device, bytecode);

			// Layout from VertexShader input signature
			var layout = new InputLayout(device, ShaderSignature.GetInputSignature(bytecode), new[] {
							new InputElement("SEMANTIC", 0, Format.R16_Float, 0, 0)
						});

			//device.
		}
	}
}
*/
