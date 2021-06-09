using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Raytracer {
	public static class SceneEditor {

		static SceneObject inspectedObject;

		public static SceneObject InspectedObject {
			get {
				return inspectedObject;
			}
			set {
				inspectedObject = value;
				UpdateInspector();
			}
		}

		static void UpdateInspector() {
			if(RaytracedRenderer.infoWindow.IsInitialized) {
				RaytracedRenderer.infoWindow.Invoke((Action)delegate {
					var inspector = RaytracedRenderer.infoWindow.propertiesPanel;
					inspector.Controls.Clear();
					if(inspectedObject == null) return;
					var set = ReflectionTest.GetExposedFieldSet(inspectedObject.GetType());
					foreach(var prop in set.fields) {
						DrawItem(prop.Key, prop.Value, inspector);
					}
				});
			}
		}

		static void DrawItem(string identifier, ReflectionTest.ExposedFieldSet.ExposedField field, FlowLayoutPanel inspector) {

			var panel = new FlowLayoutPanel();
			panel.Name = identifier;
			panel.BorderStyle = BorderStyle.FixedSingle;
			panel.FlowDirection = FlowDirection.TopDown;
			panel.AutoSize = true;
			panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel.Controls.Add(new Label() {
				Text = identifier
				//Name = field.fieldName
			});
			DrawValue(panel, field);
			inspector.Controls.Add(panel);
		}

		static void DrawValue(Panel panel, ReflectionTest.ExposedFieldSet.ExposedField value) {
			var v = value.GetValue(inspectedObject);
			if(v == null) return;
			if(v is float f) {
				panel.Controls.Add(CreateNumeric(null, f, (b) => value.SetValue(inspectedObject, b)));
			} else if(v is Vector3 v3) {
				panel.Controls.Add(CreateNumeric("X", v3.X, (b) => SetComponent(inspectedObject, value, 0, b)));
				panel.Controls.Add(CreateNumeric("Y", v3.Y, (b) => SetComponent(inspectedObject, value, 1, b)));
				panel.Controls.Add(CreateNumeric("Z", v3.Z, (b) => SetComponent(inspectedObject, value, 2, b)));
			} else if(v is Enum e) {
				panel.Controls.Add(CreateDropDown(e.ToString(), e, (b) => value.SetValue(inspectedObject, Enum.Parse(e.GetType(), b))));
			} else if(v is Color c) {
				panel.Controls.Add(CreateColorNumeric("R", c.r, (b) => SetComponent(inspectedObject, value, 0, b)));
				panel.Controls.Add(CreateColorNumeric("G", c.g, (b) => SetComponent(inspectedObject, value, 1, b)));
				panel.Controls.Add(CreateColorNumeric("B", c.b, (b) => SetComponent(inspectedObject, value, 2, b)));
				panel.Controls.Add(CreateColorNumeric("A", c.a, (b) => SetComponent(inspectedObject, value, 3, b)));
			} else {
				panel.Controls.Add(new Label() {
					Text = "Don't know " + v.GetType().Name
				});
			}
		}

		static void SetComponent(object inspected, ReflectionTest.ExposedFieldSet.ExposedField field, int index, float newvalue) {
			var v = field.GetValue(inspected);
			if(v is Vector3 v3) {
				v3 = v3.SetAxisValue(index, newvalue);
				field.SetValue(inspected, v3);
			} else if(v is Color vc) {
				vc = vc.SetComponent(index, newvalue);
				field.SetValue(inspected, vc);
			} else {
				throw new NotSupportedException();
			}
		}

		static NumericUpDown CreateNumeric(string label, float f, Action<float> feedback) {
			NumericUpDown num;
			if(!string.IsNullOrEmpty(label)) {
				num = new LabeledNumericUpDown(label);
			} else {
				num = new NumericUpDown();
			}
			num.DecimalPlaces = 3;
			num.Minimum = -999;
			num.Maximum = 999;
			num.TextAlign = HorizontalAlignment.Right;
			num.Width = 80;
			num.Value = (decimal)f;
			num.ValueChanged += (object sender, EventArgs e) => {
				feedback((float)num.Value);
				RaytracedRenderer.RedrawScreen(false);
			};
			num.Increment = 0.5m;
			return num;
		}

		static NumericUpDown CreateColorNumeric(string label, float f, Action<float> feedback) {
			var num = CreateNumeric(label, f, feedback);
			num.Minimum = 0;
			num.Maximum = 1;
			num.Increment = 0.1m;
			return num;
		}

		static ComboBox CreateDropDown(string index, Enum items, Action<string> feedback) {
			ComboBox box = new ComboBox() {
				DropDownStyle = ComboBoxStyle.DropDownList
			};
			foreach(var e in Enum.GetNames(items.GetType())) {
				box.Items.Add(e);
			}
			box.SelectedItem = index;
			box.SelectedValueChanged += (object sender, EventArgs e) => {
				feedback((string)box.SelectedItem);
				RaytracedRenderer.RedrawScreen(false);
			};
			return box;
		}
	}
}
