using System;
using System.Numerics;
using static Raytracer.RaytracerEngine;

namespace Raytracer {

	public enum QualityLevel {
		Low,
		High
	}

	public enum ShaderType {
		Default,
		SolidColor,
		DefaultCheckered,
		ReflectiveDebug,
		NormalsDebug
	}

	public enum LightingType {
		SimpleNormalBased,
		RaytracedNoShadows,
		RaytracedHardShadows
	}

	public enum TextureMappingType {
		WorldXYZ,
		WorldXProj,
		WorldYProj,
		WorldZProj,
		LocalXYZ,
		LocalXProj,
		LocalYProj,
		LocalZProj,
		Screen
	}

	public class TilingVector {
		public float x;
		public float y;
		public float width;
		public float height;
		public float angle;

		public Vector2 Offset => new Vector2(x, y);
		public Vector2 Scale => new Vector2(width, height);

		public static readonly TilingVector defaultVector = new TilingVector();

		public TilingVector() : this(0,0,1,1) {

		}

		public TilingVector(float x, float y, float w, float h, float a = 0) {
			this.x = x;
			this.y = y;
			width = w;
			height = h;
			angle = a;
		}

		public static TilingVector FromSize(float textureSize) {
			return new TilingVector(0, 0, 1f / textureSize, 1f / textureSize);
		}

		public Vector2 Apply(Vector2 uv) {
			uv += Offset;
			uv *= Scale;
			if(angle != 0) {
				uv = MathUtils.RotateAround(Vector2.Zero, angle, uv);
			}
			return uv;
		}
	}

	[ObjectIdentifier("MATERIAL")]
	public class Material {

		public static Material DefaultMaterial = new Material(Color.White, 0, 0, ShaderType.Default);

		public ShaderType shader;
		[DataIdentifier("COLOR")]
		public Color mainColor = new Color(0.5f, 0.5f, 0.5f, 1);
		[DataIdentifier("SECONDARYCOLOR")]
		public Color secColor = new Color(0.25f, 0.25f, 0.25f, 1);

		[DataIdentifier("MAINTEX")]
		public Sampler2D mainTexture;
		[DataIdentifier("TILING")]
		public TilingVector textureTiling = new TilingVector();
		[DataIdentifier("MAPPING")]
		public TextureMappingType mappingType;

		[DataIdentifier("REFL")]
		public float reflectivity = 0.5f;
		[DataIdentifier("SMOOTH")]
		public float smoothness = 1;
		public float? transparencyCutout = null;
		[DataIdentifier("REFRACTION")]
		public float refraction = 0.5f;

		public Material() {
		}

		public Material(Color c, float r, float s, ShaderType shader = ShaderType.Default) {
			mainColor = c;
			reflectivity = r;
			smoothness = s;
			this.shader = shader;
		}

		public void HandleExtraIdentifier(string extra) {
			if(extra == "D") {
				reflectivity = 0;
				smoothness = 0;
			}
		}

		public static Material CreateTexturedMaterial(string textureName, Color color, float r, float s, TilingVector tiling) {
			return new Material(color, r, s) {
				mainTexture = Sampler2D.Create(textureName),
				textureTiling = tiling
			};
		}

		public static Material CreateTexturedMaterial(string textureName, float r, float s, TilingVector tiling) {
			return CreateTexturedMaterial(textureName, Color.White, r, s, tiling);
		}

		public static Material CreateCheckerMaterial(Color c1, Color c2, float r, float s, float tiling) {
			return new Material(c1, r, s, ShaderType.DefaultCheckered) {
				secColor = c2,
				textureTiling = new TilingVector(0, 0, tiling, 1)
			};
		}

		public Color GetColor(Shape shape, Vector3 pos, Vector3 nrm, Ray ray) {
			float distance = ray.travelDistance;
			Color output;
			Color texColor;
			if(mainTexture != null) {
				var coords = GetTextureCoords(shape, pos, nrm, ray);
				coords = textureTiling.Apply(coords);
				texColor = mainTexture.Sample(coords.X, coords.Y);
			} else {
				texColor = Color.White;
			}
			if(shader == ShaderType.Default) {
				output = ReflectiveShader(texColor * mainColor, shape, nrm, ray);
			} else if(shader == ShaderType.DefaultCheckered) {
				var c = pos * textureTiling.width;
				bool alternate = c.Z % 2 == 1;
				Color col = texColor * ((c.X % 2 == c.Y % 2) ^ alternate ? secColor : mainColor);
				output = ReflectiveShader(col, shape, nrm, ray);
			} else if(shader == ShaderType.SolidColor) {
				output = mainColor;
			} else if(shader == ShaderType.ReflectiveDebug) {
				var b = MathUtils.Bounce(ray.Direction, nrm);
				output = new Color(b.X, b.Y, b.Z, 1);
			} else if(shader == ShaderType.NormalsDebug) {
				output = new Color(Math.Abs(nrm.X), Math.Abs(nrm.Y), Math.Abs(nrm.Z), 1);
			} else {
				output = Color.Black;
			}
			if(RaytracerEngine.Scene.fogDistance != null) {
				//Apply fog
				float fogDensity = Math.Min(1, distance / (float)RaytracerEngine.Scene.fogDistance);
				output = Color.Lerp(output, RaytracerEngine.Scene.fogColor, fogDensity);
			}
			return output;
		}

		private Color ReflectiveShader(Color baseColor, Shape shape, Vector3 nrm, Ray ray) {
			baseColor *= 0.75f;
			Color final = baseColor;
			var reflNrm = MathUtils.Bounce(ray.Direction, nrm);

			var shade = CalculateLighting(CurrentSettings.lightingType, ray.position, shape, nrm, reflNrm);
			final *= shade;
			//Apply transparency
			if(mainColor.a < 1) {
				var newray = new Ray(ray.position, MathUtils.Refract(ray.Direction, nrm, refraction), ray.reflectionIteration+1, ray.sourceScreenPos);
				var backColor = SceneRenderer.TraceRay(RaytracerEngine.Scene, newray, shape) * mainColor;
				final = Color.Lerp(backColor, final, mainColor.a);
			}
			//Apply reflections
			if(reflectivity > 0 && ray.reflectionIteration <= CurrentSettings.maxBounces) {
				var newray = new Ray(ray.position, reflNrm, ray.reflectionIteration + 1, ray.sourceScreenPos);
				var reflColor = SceneRenderer.TraceRay(RaytracerEngine.Scene, newray, shape) * reflectivity;
				final += reflColor;
			}
			return final;
		}

		private Color CalculateLighting(LightingType lighting, Vector3 point, Shape shape, Vector3 nrm, Vector3 reflNrm) {
			if(lighting == LightingType.SimpleNormalBased) {
				//Apply simple shading
				float x = 0.5f + nrm.X * 0.5f;
				float y = 0.5f + nrm.Y * 0.5f;
				float z = 0.5f + nrm.Z * -0.5f;
				float brightness = MathUtils.Step(Math.Max(0, Math.Min(1, y * 0.6f + z * 0.25f + x * 0.15f)), 0.33f);
				return Color.Lerp(RaytracerEngine.Scene.ambientColor, RaytracerEngine.Scene.simpleSunColor, brightness);
			} else {
				//Apply "complex" lighting
				Color lightCol = RaytracerEngine.Scene.ambientColor;
				foreach(var l in RaytracerEngine.Scene.GetContributingLights(point)) {
					lightCol += l.GetLightAtPoint(RaytracerEngine.Scene, point, nrm, lighting, shape, out bool shadow);
					if(CurrentSettings.specularHighlights && !shadow) {
						lightCol += l.GetSpecularHighlight(point, reflNrm, smoothness);
					}
				}
				return lightCol;
			}
		}

		private Vector2 GetTextureCoords(Shape shape, Vector3 pos, Vector3 nrm, Ray ray) {
			Vector2 uv;
			if(mappingType == TextureMappingType.LocalXYZ ||
				mappingType == TextureMappingType.LocalXProj ||
				mappingType == TextureMappingType.LocalYProj ||
				mappingType == TextureMappingType.LocalZProj)
			{
				pos += shape.WorldPosition;
			}
			switch(mappingType) {
				case TextureMappingType.LocalXYZ:
				case TextureMappingType.WorldXYZ: uv = MapWorld(pos, nrm, true); break;
				case TextureMappingType.LocalXProj:
				case TextureMappingType.WorldXProj: uv = new Vector2(pos.Z, pos.Y); break;
				case TextureMappingType.LocalYProj:
				case TextureMappingType.WorldYProj: uv = new Vector2(pos.X, pos.Z); break;
				case TextureMappingType.LocalZProj:
				case TextureMappingType.WorldZProj: uv = new Vector2(pos.X, pos.Y); break;
				case TextureMappingType.Screen: uv = ray.sourceScreenPos; break;
				default: uv = Vector2.Zero; break;
			}
			return uv;
		}

		private Vector2 MapWorld(Vector3 pos, Vector3 nrm, bool correctMirroring) {
			if(correctMirroring) {
				Vector3 mirrorCorrection = new Vector3(nrm.X < 0 ? -1 : 1, nrm.Y < 0 ? -1 : 1, nrm.Z < 0 ? -1 : 1);
				pos *= mirrorCorrection;
			}
			nrm = nrm.Abs();
			var max = Math.Max(nrm.X, Math.Max(nrm.Y, nrm.Z));
			if(max == nrm.X) {
				return new Vector2(pos.Z, pos.Y);
			} else if(max == nrm.Y) {
				return new Vector2(pos.X, pos.Z);
			} else {
				return new Vector2(pos.X, pos.Y);
			}
		}
	}
}
