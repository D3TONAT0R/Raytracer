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

		static object inspectedObject;

		public static object InspectedObject
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
					var set = Reflector.GetExposedFieldSet(inspectedObject.GetType());
					foreach(var prop in set.fields)
					{
						DrawItem(prop.Key, prop.Value, inspector);
					}
				});
			}
		}

		static void DrawItem(string identifier, Reflector.ExposedFieldSet.ExposedField field, FlowLayoutPanel inspector)
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

		static void DrawValue(Panel panel, Reflector.ExposedFieldSet.ExposedField value)
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
				panel.Controls.Add(CreateIntNumeric(null, i, null, null, (x) => value.SetValue(inspectedObject, x)));
			}
			else if(v is float f)
			{
				panel.Controls.Add(CreateNumeric(null, f, null, null, value.attribute.numericIncrement, (x) => value.SetValue(inspectedObject, x)));
			}
			else if(v is Vector3 v3)
			{
				panel.Controls.Add(CreateNumeric("X", v3.X, null, null, value.attribute.numericIncrement, (x) => SetComponent(inspectedObject, value, 0, x)));
				panel.Controls.Add(CreateNumeric("Y", v3.Y, null, null, value.attribute.numericIncrement, (x) => SetComponent(inspectedObject, value, 1, x)));
				panel.Controls.Add(CreateNumeric("Z", v3.Z, null, null, value.attribute.numericIncrement, (x) => SetComponent(inspectedObject, value, 2, x)));
			}
			else if(v is Enum e)
			{
				panel.Controls.Add(CreateDropDown(e.ToString(), e, (x) => value.SetValue(inspectedObject, Enum.Parse(e.GetType(), x))));
			}
			else if(v is Color c)
			{
				panel.Controls.Add(CreateColorNumeric("R", c.r, 1f, value.attribute.numericIncrement, (x) => SetComponent(inspectedObject, value, 0, x)));
				panel.Controls.Add(CreateColorNumeric("G", c.g, 1f, value.attribute.numericIncrement, (x) => SetComponent(inspectedObject, value, 1, x)));
				panel.Controls.Add(CreateColorNumeric("B", c.b, 1f, value.attribute.numericIncrement, (x) => SetComponent(inspectedObject, value, 2, x)));
				panel.Controls.Add(CreateColorNumeric("A", c.a, 1f, value.attribute.numericIncrement, (x) => SetComponent(inspectedObject, value, 3, x)));
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

		static void SetComponent(object inspected, Reflector.ExposedFieldSet.ExposedField field, int index, float newvalue)
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

		static NumericUpDown CreateNumeric(string label, float f, float? min, float? max, float? increment, Action<float> feedback)
		{
			NumericUpDown num = new LabeledNumericUpDown(label);
			num.DecimalPlaces = 3;
			num.InterceptArrowKeys = false;
			num.Increment = increment.HasValue ? (decimal)increment : 0.1m;
			num.TextAlign = HorizontalAlignment.Right;
			if(min.HasValue) num.Minimum = (decimal)min;
			else num.Minimum = decimal.MinValue;
			if(max.HasValue) num.Maximum = (decimal)max;
			else num.Maximum = decimal.MaxValue;
			num.Width = 80;
			num.Value = (decimal)f;
			num.ValueChanged += (object sender, EventArgs e) =>
			{
				feedback((float)num.Value);
				RaytracerEngine.RedrawScreen(false);
			};
			return num;
		}

		static NumericUpDown CreateIntNumeric(string label, int i, int? min, int? max, Action<int> feedback)
		{
			var num = CreateNumeric(label, i, min, max, 1, (f) => feedback?.Invoke((int)f));
			num.DecimalPlaces = 0;
			num.Increment = 1;
			return num;
		}

		static NumericUpDown CreateColorNumeric(string label, float f, float limit, float? increment, Action<float> feedback)
		{
			return CreateNumeric(label, f, 0f, limit, increment ?? 0.01f, feedback);
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
