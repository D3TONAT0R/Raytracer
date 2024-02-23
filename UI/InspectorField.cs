using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Raytracer
{
	public class InspectorField : FlowLayoutPanel
	{
		private Label label;
		private FlowLayoutPanel values;

		public InspectorField(string text) : base()
		{
			Name = text;

			BorderStyle = BorderStyle.FixedSingle;
			FlowDirection = FlowDirection.TopDown;
			AutoSize = true;
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
			Anchor = AnchorStyles.Left | AnchorStyles.Right;
			Margin = new Padding(0, 0, 0, 2);

			label = new Label
			{
				Text = text,
				Anchor = AnchorStyles.Left | AnchorStyles.Right,
				Height = 16,
				Padding = new Padding(0, 3, 0, 0),
				AutoSize = true,
				AutoEllipsis = true
			};
			Controls.Add(label);
			label.BringToFront();

			values = new FlowLayoutPanel()
			{
				FlowDirection = FlowDirection.LeftToRight,
				Padding = new Padding(0),
				BorderStyle = BorderStyle.None,
				AutoSize = true,
				AutoSizeMode = AutoSizeMode.GrowAndShrink,
				Anchor = AnchorStyles.Left | AnchorStyles.Right
			};
			Controls.Add(values);
		}

		public void AddControl(Control control)
		{
			values.Controls.Add(control);
		}
	}
}
