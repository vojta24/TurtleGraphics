using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Media;

namespace TurtleGraphics {
	public class ColorData : ParsedData {

		private readonly char[] HEX = new[] { '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
		private readonly Random _random;

		public ColorData(string inputColor, Dictionary<string, object> variables) : base(inputColor.Trim()) {
			_random = new Random((int)DateTime.Now.Ticks);
			Variables = variables;
		}

		public override bool IsBlock => false;

		public override ParsedAction Action => ParsedAction.Color;

		public override TurtleData Compile(TurtleData previous, CancellationToken token) {
			token.ThrowIfCancellationRequested();
			Brush brush = (Brush)new BrushConverter().ConvertFromString(Arg1 == "random" ? RandColor() : Arg1);
			brush.Freeze();

			return new TurtleData {
				Angle = previous.Angle,
				SetAngle = previous.SetAngle,
				Brush = brush,
				BrushThickness = previous.BrushThickness,
				MoveTo = previous.MoveTo,
				PenDown = previous.PenDown,
				Jump = false,
				Action = Action,
			};
		}

		public override IList<TurtleData> CompileBlock(TurtleData previous, CancellationToken token) {
			throw new NotImplementedException();
		}

		private string RandColor() {
			StringBuilder builder = new StringBuilder("#");

			for (int i = 0; i < 6; i++) {
				char c = HEX[_random.Next(0, HEX.Length)];
				builder.Append(c);
			}
			return builder.ToString();
		}
	}
}