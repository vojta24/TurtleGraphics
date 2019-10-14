using System;
using System.Windows;
using System.Windows.Media;

namespace TurtleGraphics {
	[Serializable]
	public struct TurtleData {
		public static TurtleData NoAction => new TurtleData() { Action = ParsedAction.NONE };
		public double Distance { get; set; }
		public double Angle { get; set; }
		public bool SetAngle { get; set; }
		public bool PopPosition { get; set; }
		public Point MoveTo { get; set; }

		[NonSerialized]
		private Brush _brush;
		public Brush Brush {
			get => _brush ?? (_brush = (SolidColorBrush)new BrushConverter().ConvertFromString(SerializedBrush));
			set { _brush = value; SerializedBrush = new BrushConverter().ConvertToString(value); }
		}

		public string SerializedBrush { get; set; }
		public double BrushThickness { get; set; }
		public bool PenDown { get; set; }
		public ParsedAction Action { get; set; }
		public PenLineCap LineCap { get; set; }
	}
}
