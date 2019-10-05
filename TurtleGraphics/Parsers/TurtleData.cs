using System.Windows;
using System.Windows.Media;

namespace TurtleGraphics {
	public struct TurtleData {
		public static TurtleData NoAction => new TurtleData() { Action = ParsedAction.NONE };
		public double Distance { get; set; }
		public double Angle { get; set; }
		public bool SetAngle { get; set; }
		public bool PopPosition { get; set; }
		public Point MoveTo { get; set; }
		public Brush Brush { get; set; }
		public double BrushThickness { get; set; }
		public bool PenDown { get; set; }
		public bool Jump { get; set; }
		public ParsedAction Action { get; set; }
		public PenLineCap LineCap { get; set; }
	}
}
