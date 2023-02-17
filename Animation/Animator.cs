using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {
	public class Animator {

		public float CurrentTime { get; private set; } = 0f;
		public int CurrentFrame => (int)(CurrentTime * framesPerSecond);

		public List<AnimatedProperty> animatedProperties = new List<AnimatedProperty>();

		public float recStartTime = 0f;
		public float? recEndTime = null;
		public float framesPerSecond = 15;

		public float RecEndTime => recEndTime ?? RecDuration;
		public float RecDuration => RecEndTime - recStartTime;
		public int TotalRecFrameCount => (int)Math.Ceiling(RecDuration * framesPerSecond);
		public bool HasNextFrame => CurrentTime < RecEndTime;


		public float TotalAnimDuration
		{
			get
			{
				float l = 0;
				foreach(var p in animatedProperties)
				{
					l = Math.Max(l, p.Duration);
				}
				return l;
			}
		}

		public Animator(float startTime, float? endTime, float fps)
		{
			recStartTime = startTime;
			recEndTime = endTime;
			framesPerSecond = fps;
		}


		public void Init(Scene scene)
		{
			foreach(var prop in animatedProperties)
			{
				prop.Init(scene);
			}
		}

		public void SetTime(float time)
		{
			CurrentTime = time;
			foreach(var prop in animatedProperties)
			{
				prop.SampleAnimation(CurrentTime);
			}
		}

		public void Rewind()
		{
			SetTime(recStartTime);
		}

		public void Step(float delta)
		{
			SetTime(CurrentTime + delta);
		}

		public void StepFrames(float frames = 1)
		{
			Step(frames / framesPerSecond);
		}
	}
}
