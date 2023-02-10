using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Raytracer
{
	public static class SceneEditor
	{

		static SceneObject inspectedObject;

		public static SceneObject InspectedObject
		{
			get
			{
				return inspectedObject;
			}
			set
			{
				inspectedObject = value;
				UpdateInspector();
			}
		}

		static void UpdateInspector()
		{
			if(RaytracerEngine.infoWindow.IsInitialized)
			{
				RaytracerEngine.infoWindow.Invoke((Action)delegate
				{
					var inspector = RaytracerEngine.infoWindow.propertiesPanel;
					inspector.Controls.Clear();
					if(inspectedObject == null) return;
					var set = Reflection.GetExposedFieldSet(inspectedObject.GetType());
					foreach(var prop in set.fields)
					{
						DrawItem(prop.Key, prop.Value, inspector);
					}
				});
			}
		}

		static void DrawItem(string identifier, Reflection.ExposedFieldSet.ExposedField field, FlowLayoutPanel inspector)
		{

			var panel = new FlowLayoutPanel();
			panel.Name = identifier;
			panel.BorderStyle = BorderStyle.FixedSingle;
			panel.FlowDirection = FlowDirection.TopDown;
			panel.AutoSize = true;
			panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel.Controls.Add(new Label()
			{
				Text = identifier
				//Name = field.fieldName
			});
			DrawValue(panel, field);
			inspector.Controls.Add(panel);
		}

		static void DrawValue(Panel panel, Reflection.ExposedFieldSet.ExposedField value)
		{
			var v = value.GetValue(inspectedObject);
			if(v == null) return;
			if(v is bool b)
			{
				var cb = new CheckBox()
				{
					TextAlign = System.Drawing.ContentAlignment.MiddleRight,
					Width = 80,
					Checked = b
				};
				cb.CheckStateChanged += (object sender, EventArgs e) =>
				{
					value.SetValue(inspectedObject, cb.Checked);
					RaytracerEngine.RedrawScreen(false);
				};
				panel.Controls.Add(cb);
			}
			else if(v is int i)
			{
				panel.Controls.Add(CreateIntNumeric(null, i, (x) => value.SetValue(inspectedObject, x)));
			}
			else if(v is float f)
			{
				panel.Controls.Add(CreateNumeric(null, f, (x) => value.SetValue(inspectedObject, x)));
			}
			else if(v is Vector3 v3)
			{
				panel.Controls.Add(CreateNumeric("X", v3.X, (x) => SetComponent(inspectedObject, value, 0, x)));
				panel.Controls.Add(CreateNumeric("Y", v3.Y, (x) => SetComponent(inspectedObject, value, 1, x)));
				panel.Controls.Add(CreateNumeric("Z", v3.Z, (x) => SetComponent(inspectedObject, value, 2, x)));
			}
			else if(v is Enum e)
			{
				panel.Controls.Add(CreateDropDown(e.ToString(), e, (x) => value.SetValue(inspectedObject, Enum.Parse(e.GetType(), x))));
			}
			else if(v is Color c)
			{
				panel.Controls.Add(CreateColorNumeric("R", c.r, (x) => SetComponent(inspectedObject, value, 0, x)));
				panel.Controls.Add(CreateColorNumeric("G", c.g, (x) => SetComponent(inspectedObject, value, 1, x)));
				panel.Controls.Add(CreateColorNumeric("x", c.b, (x) => SetComponent(inspectedObject, value, 2, x)));
				panel.Controls.Add(CreateColorNumeric("A", c.a, (x) => SetComponent(inspectedObject, value, 3, x)));
			}
			else
			{
				/*
				panel.Controls.Add(new Label()
				{
					Text = "Don't know " + v.GetType().Name
				});
				*/
			}
		}

		static void SetComponent(object inspected, Reflection.ExposedFieldSet.ExposedField field, int index, float newvalue)
		{
			var v = field.GetValue(inspected);
			if(v is Vector3 v3)
			{
				v3 = v3.SetAxisValue(index, newvalue);
				field.SetValue(inspected, v3);
			}
			else if(v is Color vc)
			{
				vc = vc.SetComponent(index, newvalue);
				field.SetValue(inspected, vc);
			}
			else
			{
				throw new NotSupportedException();
			}
		}

		static NumericUpDown CreateNumeric(string label, float f, Action<float> feedback)
		{
			NumericUpDown num;
			if(!string.IsNullOrEmpty(label))
			{
				num = new LabeledNumericUpDown(label);
			}
			else
			{
				num = new NumericUpDown();
			}
			num.DecimalPlaces = 3;
			num.Minimum = -999;
			num.Maximum = 999;
			num.TextAlign = HorizontalAlignment.Right;
			num.Width = 80;
			num.Value = (decimal)f;
			num.ValueChanged += (object sender, EventArgs e) =>
			{
				feedback((float)num.Value);
				RaytracerEngine.RedrawScreen(false);
			};
			num.Increment = 0.5m;
			return num;
		}

		static NumericUpDown CreateIntNumeric(string label, int i, Action<int> feedback)
		{
			var num = CreateNumeric(label, i, (f) => feedback?.Invoke((int)f));
			num.DecimalPlaces = 0;
			num.Increment = 1;
			return num;
		}

		static NumericUpDown CreateColorNumeric(string label, float f, Action<float> feedback)
		{
			var num = CreateNumeric(label, f, feedback);
			num.Minimum = 0;
			num.Maximum = 1;
			num.Increment = 0.1m;
			return num;
		}

		static ComboBox CreateDropDown(string index, Enum items, Action<string> feedback)
		{
			ComboBox box = new ComboBox()
			{
				DropDownStyle = ComboBoxStyle.DropDownList
			};
			foreach(var e in Enum.GetNames(items.GetType()))
			{
				box.Items.Add(e);
			}
			box.SelectedItem = index;
			box.SelectedValueChanged += (object sender, EventArgs e) =>
			{
				feedback((string)box.SelectedItem);
				RaytracerEngine.RedrawScreen(false);
			};
			return box;
		}
	}
}
