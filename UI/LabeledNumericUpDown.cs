using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Raytracer {
	public class LabeledNumericUpDown : NumericUpDown {

        Label label;

		public LabeledNumericUpDown(string text) : base() {
            label = new Label();
            label.Text = text;
            Controls.Add(label);
            label.BringToFront();
            UpdateLabel();
        }

        public void UpdateLabel() {
            System.Drawing.Size size = TextRenderer.MeasureText(label.Text, label.Font);
            label.Padding = new Padding(0, 0, 0, 3);
            label.Size = new System.Drawing.Size(size.Width, this.Height);
            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            label.BackColor = System.Drawing.Color.Transparent;
            label.Location = new System.Drawing.Point(0, 0);
        }

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			HandledMouseEventArgs hme = e as HandledMouseEventArgs;
			if(hme != null)
				hme.Handled = true;

            var newValue = Value;
			if(e.Delta > 0)
				newValue += Increment;
			else if(e.Delta < 0)
				newValue -= Increment;
            Value = Math.Min(Maximum, Math.Max(Minimum, newValue));
		}
	}
}
