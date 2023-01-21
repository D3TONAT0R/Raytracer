﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
	public static class PersistentPrefs
	{
		public class LastSessionInfo
		{
			public string sceneFile;
			public Vector3 cameraPos;
			public Vector3 cameraRot;
			public float cameraFOV;
		}

		const string rootRegistryPath = @"SOFTWARE\Raytracer";

		public static bool TryGetLastSessionInfo(out LastSessionInfo info)
		{
			var key = Registry.CurrentUser.OpenSubKey(Path.Combine(rootRegistryPath, "LastSession"));
			if(key != null)
			{
				info = new LastSessionInfo();
				info.sceneFile = (string)key.GetValue("SceneFile");
				info.cameraPos = RegToVector3((string)key.GetValue("CameraPos"));
				info.cameraRot = RegToVector3((string)key.GetValue("CameraRot"));
				info.cameraFOV = float.Parse((string)key.GetValue("CameraFOV"));
				return true;
			}
			else
			{
				info = null;
				return false;
			}
		}

		public static void WriteLastSessionInfo()
		{
			if(RaytracerEngine.Scene != null)
			{
				var sn = RaytracerEngine.Scene;
				var key = Registry.CurrentUser.CreateSubKey(Path.Combine(rootRegistryPath, "LastSession"));
				key.SetValue("SceneFile", sn.sourceFile);
				var cam = Camera.MainCamera;
				key.SetValue("CameraPos", Vector3ToReg(cam.localPosition));
				key.SetValue("CameraRot", Vector3ToReg(cam.rotation));
				key.SetValue("CameraFOV", cam.fieldOfView.ToString());
			}
		}

		private static string Vector3ToReg(Vector3 v)
		{
			return $"{v.X};{v.Y};{v.Z}";
		}

		private static Vector3 RegToVector3(string r)
		{
			var s = r.Split(';');
			return new Vector3(
				float.Parse(s[0]),
				float.Parse(s[1]),
				float.Parse(s[2])
			);
		}
	}
}
