using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {

	[ObjectIdentifier("LIGHT")]
	public class Light : SceneObject {

		public enum LightType {
			Point,
			Directional
		}

		[DataIdentifier("LIGHTTYPE")]
		public LightType type;
		[DataIdentifier("INTENSITY")]
		public float intensity;
		[DataIdentifier("COLOR")]
		public Color color;
		[DataIdentifier("SHADOWOFFSET")]
		public float shadowStartOffset = 0;

		//Artistic overrides (not realistic)
		[DataIdentifier("SHADOWINTENSITY")]
		public float shadowIntensity = 1;
		[DataIdentifier("SHADOWCOLOR")]
		public Color shadowColor = Color.Black;

		//Directional Light properties
		[DataIdentifier("DIRECTION")]
		public Vector3 lightDirection;

		//Point Light properties
		[DataIdentifier("RANGE")]
		public float range;

		public Light() : base(null) { }

		public Light(string name) : base(name) {

		}

		public static Light CreatePointLight(Vector3 pos, float radius, float lightIntensity, Color lightColor) {
			return new Light("light_point") {
				type = LightType.Point,
				localPosition = pos,
				range = radius,
				intensity = lightIntensity,
				color = lightColor
			};
		}

		public static Light CreateDirectionalLight(Vector3 direction, float lightIntensity, Color lightColor) {
			return new Light("light_directional") {
				type = LightType.Directional,
				lightDirection = direction,
				intensity = lightIntensity,
				color = lightColor
			};
		}

		public bool Contributes(Vector3 point) {
			if(intensity <= 0) return false;
			if(type == LightType.Directional) {
				return true;
			} else {
				return Vector3.Distance(WorldPosition, point) <= range;
			}
		}

		public Color GetLightAtPoint(Scene scene, Vector3 point, Vector3 normal, LightingType lighting, Shape sourceShape, out bool shadowed) {
			var targetPoint = point;
			//var localPoint = point - HierarchyPositionOffset;
			//point -= HierarchyPositionOffset;
			if(lighting == LightingType.SimpleNormalBased) {
				shadowed = false;
				return Color.Black;
			} else if(lighting == LightingType.RaytracedNoShadows) {
				shadowed = false;
				return color * GetIlluminationAtPoint(point, normal, GetLightNormal(point));
			} else if(lighting == LightingType.RaytracedHardShadows) {
				//return new Color(point.X - (int)point.X, point.Y - (int)point.Y, point.Z - (int)point.Z);
				var lightNormal = GetLightNormal(point);
				if(RaytracerEngine.CurrentRenderSettings.allowSelfShadowing) {
					//TODO: may break lighting, rewrite needed
					//Depenetrate surface to allow self shadowing
					//point += sourceShape.GetSurfaceProximity(point) * (normal * 1.1f);
					sourceShape = null;
				}
				var illum = GetIlluminationAtPoint(point, normal, lightNormal);
				Vector3? caster = null;
				/*bool castShadowRay;
				if(type == LightType.Directional)
				{
					castShadowRay = true;
				}
				else
				{
					castShadowRay = Vector3.Distance(WorldPosition, point) > shadowStartOffset;
				}
				if(castShadowRay)
				{*/
				Ray ray;
				if (type == LightType.Directional)
				{
					ray = new Ray(point, lightNormal, 0, Vector2.Zero);
				}
				else
				{
					ray = new Ray(point, lightNormal, 0, Vector2.Zero, Vector3.Distance(WorldPosition, point) - shadowStartOffset);
				}
				if (ray.maxDistance > 0)
				{
					caster = SceneRenderer.TraceRay(scene, ref ray, out _, sourceShape);
				}

				//}
				float shadow;
				if(caster == null) {
					//The point is not shadowed
					shadowed = false;
					shadow = 0;
				} else {
					//The point is shadowed
					shadowed = true;
					shadow = shadowIntensity;
				}
				return Color.Lerp(color, shadowColor, shadow) * illum;
			}
			shadowed = false;
			return Color.Magenta;
		}

		public Color GetSpecularHighlight(Vector3 point, Vector3 reflNormal, float spec) {
			var lnorm = Vector3.Normalize(localPosition - point);
			var dev = 0.5f + Vector3.Dot(reflNormal, lnorm) * 0.5f;
			//float l = (float)Math.Max(0, Math.Pow(dev, 80f * reflectivity));
			float l = Math.Max(0, MathUtils.Remap(0.5f+spec*0.48f, 1f, 1-dev));
			return color * intensity * l * spec;
		}

		private Vector3 GetLightNormal(Vector3 pos) {
			if(type == LightType.Directional) {
				return -Vector3.Normalize(lightDirection);
			} else {
				return Vector3.Normalize(WorldPosition - pos);
			}
		}

		public float GetIlluminationAtPoint(Vector3 point, Vector3 normal, Vector3 lightNormal) {
			float dot;
			float illum = intensity;
			if(type == LightType.Directional) {
				dot = Vector3.Dot(normal, lightNormal);
			} else {
				dot = Vector3.Dot(normal, lightNormal);
				float falloff = Math.Max(0, 1f - (Vector3.Distance(point, WorldPosition) / range));
				illum *= falloff;
			}
			return illum * Math.Max(0, dot);
		}
	}
}
