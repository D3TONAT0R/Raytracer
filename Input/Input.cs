using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Raytracer {
	public static class Input {

		public static InputKey esc = new InputKey(Key.Escape);
		public static InputKey confirm = new InputKey(Key.Enter);
		public static InputKey space = new InputKey(Key.Space);
		public static InputKey tab = new InputKey(Key.Tab);
		public static InputKey up = new InputKey(Key.Up, Key.W);
		public static InputKey w = new InputKey(Key.W);
		public static InputKey arrowUp = new InputKey(Key.Up);
		public static InputKey down = new InputKey(Key.Down, Key.S);
		public static InputKey s = new InputKey(Key.S);
		public static InputKey arrowDown = new InputKey(Key.Down);
		public static InputKey left = new InputKey(Key.Left, Key.A);
		public static InputKey a = new InputKey(Key.A);
		public static InputKey arrowLeft = new InputKey(Key.Left);
		public static InputKey right = new InputKey(Key.Right, Key.D);
		public static InputKey d = new InputKey(Key.D);
		public static InputKey arrowRight = new InputKey(Key.Right);
		public static InputKey q = new InputKey(Key.PageUp, Key.Q);
		public static InputKey e = new InputKey(Key.PageDown, Key.E);
		public static InputKey comma = new InputKey(Key.OemComma);
		public static InputKey period = new InputKey(Key.OemPeriod);
		public static InputKey r = new InputKey(Key.R);
		public static InputKey o = new InputKey(Key.O);
		public static InputKey p = new InputKey(Key.P);
		public static InputKey l = new InputKey(Key.L);
		public static InputKey y = new InputKey(Key.Y);
		public static InputKey x = new InputKey(Key.X);
		public static InputKey b = new InputKey(Key.B);
		public static InputKey n = new InputKey(Key.N);
		public static InputKey m = new InputKey(Key.M);
		public static InputKey k = new InputKey(Key.K);

		public static InputKey nLeft = new InputKey(Key.NumPad4);
		public static InputKey nRight = new InputKey(Key.NumPad6);
		public static InputKey nDown = new InputKey(Key.NumPad2);
		public static InputKey nUp = new InputKey(Key.NumPad8);

		public static InputKey[] allKeys = new InputKey[] {
			esc,confirm,space,tab,up,w,arrowUp,down,s,arrowDown,left,a,arrowLeft,right,d,arrowRight,q,e,comma,period,r,nLeft,nRight,nDown,nUp,b,k
		};

		public static void Update() {
			esc.Evaluate();
			confirm.Evaluate();
			space.Evaluate();
			tab.Evaluate();
			up.Evaluate();
			w.Evaluate();
			arrowUp.Evaluate();
			down.Evaluate();
			s.Evaluate();
			arrowDown.Evaluate();
			left.Evaluate();
			a.Evaluate();
			arrowLeft.Evaluate();
			right.Evaluate();
			d.Evaluate();
			arrowRight.Evaluate();
			q.Evaluate();
			e.Evaluate();
			comma.Evaluate();
			period.Evaluate();
			r.Evaluate();
			o.Evaluate();
			p.Evaluate();
			l.Evaluate();
			y.Evaluate();
			x.Evaluate();
			k.Evaluate();
			nLeft.Evaluate();
			nRight.Evaluate();
			nDown.Evaluate();
			nUp.Evaluate();
			b.Evaluate();
			n.Evaluate();
			m.Evaluate();
		}

		public static void CancelAll() {
			foreach(var k in allKeys) {
				k.isDown = false;
				k.isUp = false;
				k.isPressed = false;
			}
		}
	}
}
