using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ConsoleGameEngine {
	public class InputKey {

		public Key[] keys;

		public bool isDown;
		public bool isPressed;
		public bool isUp;

		private bool wasPressed;

		public InputKey(params Key[] k) {
			keys = k;
		}

		public void Evaluate() {
			bool down = false;
			foreach (Key k in keys) down |= Keyboard.IsKeyDown(k);
			isPressed = down;
			isDown = down && !wasPressed;
			isUp = !isPressed;
			wasPressed = down;
		}
	}
}
