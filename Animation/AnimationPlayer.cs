using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.Animation
{
	public class AnimationPlayer
	{
		public float startTime = 0f;
		public float? endTime = null;
		public float framesPerSecond = 15;

		public float ActualEndTime => endTime ?? Animator.Duration;
		public float Duration => ActualEndTime - startTime;
		public int TotalFrameCount => (int)Math.Ceiling(Duration * framesPerSecond);
		public bool HasNextFrame => Animator.CurrentTime < ActualEndTime;

		public AnimationPlayer(float startTime, float? endTime, float framesPerSecond)
		{
			this.startTime = startTime;
			this.endTime = endTime;
			this.framesPerSecond = framesPerSecond;
		}

		public void Rewind()
		{
			Animator.SetTime(startTime);
		}

		public void Step(float frames = 1)
		{
			Animator.Step(frames / framesPerSecond);
		}
	}
}
