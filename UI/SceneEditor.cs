using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
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
					if(inspectedObject == null)
					{
						return;
					}
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
			var panel = new InspectorField(identifier);
			DrawValue(panel, field);
			inspector.Controls.Add(panel);
		}

		static void DrawValue(InspectorField panel, Reflector.ExposedFieldSet.ExposedField value)
		{
			var v = value.GetValue(inspectedObject);
			if(v == null) return;
			if(v is bool b)
			{
				panel.AddControl(CreateCheckBox(null, b, (x) => value.SetValue(inspectedObject, x)));
			}
			else if(v is int i)
			{
				panel.AddControl(CreateIntNumeric(null, i, null, null, (x) => value.SetValue(inspectedObject, x)));
			}
			else if(v is float f)
			{
				panel.AddControl(CreateNumeric(null, f, null, null, value.attribute.numericIncrement, (x) => value.SetValue(inspectedObject, x)));
			}
			else if(v is Vector3 v3)
			{
				panel.AddControl(CreateNumeric("X", v3.X, null, null, value.attribute.numericIncrement, (x) => SetComponent(inspectedObject, value, 0, x)));
				panel.AddControl(CreateNumeric("Y", v3.Y, null, null, value.attribute.numericIncrement, (x) => SetComponent(inspectedObject, value, 1, x)));
				panel.AddControl(CreateNumeric("Z", v3.Z, null, null, value.attribute.numericIncrement, (x) => SetComponent(inspectedObject, value, 2, x)));
			}
			else if(v is Enum e)
			{
				if(e.GetType().GetCustomAttribute<FlagsAttribute>() != null)
				{
					int flagCount = 0;
					foreach(var ev in Enum.GetValues(e.GetType()))
					{
						flagCount = Math.Max(flagCount, (int)Math.Log((int)ev, 2));
					}
					for(int j = 0; j <= flagCount; j++)
					{
						var name = Enum.GetName(e.GetType(), 1 << j);
						bool state = ((int)v & (1 << j)) != 0;
						int pos = j;
						panel.AddControl(CreateCheckBox(name, state, (s) =>
						{
							value.SetValue(inspectedObject, SetBit((int)v, pos, s));
						}));
					}
				}
				else
				{
					panel.AddControl(CreateDropDown(e.ToString(), e, (x) => value.SetValue(inspectedObject, Enum.Parse(e.GetType(), x))));
				}
			}
			else if(v is Color c)
			{
				panel.AddControl(CreateColorNumeric("R", c.r, 1f, value.attribute.numericIncrement, (x) => SetComponent(inspectedObject, value, 0, x)));
				panel.AddControl(CreateColorNumeric("G", c.g, 1f, value.attribute.numericIncrement, (x) => SetComponent(inspectedObject, value, 1, x)));
				panel.AddControl(CreateColorNumeric("B", c.b, 1f, value.attribute.numericIncrement, (x) => SetComponent(inspectedObject, value, 2, x)));
				panel.AddControl(CreateColorNumeric("A", c.a, 1f, value.attribute.numericIncrement, (x) => SetComponent(inspectedObject, value, 3, x)));
			}
			else if(v is TilingVector t)
			{
				panel.AddControl(CreateNumeric("X", t.x, null, null, 0.1f, (x) => SetComponent(inspectedObject, value, 0, x)));
				panel.AddControl(CreateNumeric("Y", t.y, null, null, 0.1f, (x) => SetComponent(inspectedObject, value, 1, x)));
				panel.AddControl(CreateNumeric("W", t.width, null, null, 0.1f, (x) => SetComponent(inspectedObject, value, 2, x)));
				panel.AddControl(CreateNumeric("H", t.height, null, null, 0.1f, (x) => SetComponent(inspectedObject, value, 3, x)));
				panel.AddControl(CreateNumeric("A", t.angle, null, null, 5.0f, (x) => SetComponent(inspectedObject, value, 4, x)));
			}
			else
			{
				/*
				panel.AddControl(new Label()
				{
					Text = "Don't know " + v.GetType().Name
				});
				*/
			}
		}

		private static CheckBox CreateCheckBox(string label, bool b, Action<bool> feedback)
		{
			var cb = new CheckBox()
			{
				TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
				Width = 80,
				Checked = b,
				Text = label,
				Height = 16,
				Margin = new Padding(0)
			};
			cb.CheckStateChanged += (object sender, EventArgs e) =>
			{
				feedback(cb.CheckState == CheckState.Checked);
				RaytracerEngine.RedrawScreen(false);
			};
			return cb;
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
			else if(v is TilingVector tv)
			{
				tv = tv.SetComponent(index, newvalue);
				field.SetValue(inspected, tv);
			}
			else
			{
				throw new NotSupportedException();
			}
		}

		static int SetBit(int value, int pos, bool state)
		{
			if(state) return value | 1 << pos;
			else return value & ~(1 << pos);
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
			var n = CreateNumeric(label, f, 0f, limit, increment ?? 0.01f, feedback);
			n.Width = 60;
			return n;
		}

		static ComboBox CreateDropDown(string selected, Enum items, Action<string> feedback)
		{
			ComboBox box = new ComboBox()
			{
				DropDownStyle = ComboBoxStyle.DropDownList,
				Width = 100,
				Anchor = AnchorStyles.Left | AnchorStyles.Right
			};
			foreach(var e in Enum.GetNames(items.GetType()))
			{
				box.Items.Add(e);
			}
			box.SelectedItem = selected;
			box.SelectedValueChanged += (object sender, EventArgs e) =>
			{
				feedback((string)box.SelectedItem);
				RaytracerEngine.RedrawScreen(false);
			};
			return box;
		}
	}
}
