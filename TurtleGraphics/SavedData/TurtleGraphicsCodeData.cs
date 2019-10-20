using System;

namespace TurtleGraphics {
	[Serializable]
	public class TurtleGraphicsCodeData {
		public TurtleData[] Data { get; set; }
		public bool ShowTurtle { get; set; }
		public bool AnimatePath { get; set; }
		public int PathAnimationSpeed { get; set; }
		public int TurtleSpeed { get; set; }
		public string BackgroundColor { get; set; }
	}
}
