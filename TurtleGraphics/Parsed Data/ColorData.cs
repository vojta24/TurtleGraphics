using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TurtleGraphics {
	public class ColorData : ParsedData {

		private readonly char[] HEX = new[] { '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
		private readonly Random _random;
		private readonly MainWindow _window;

		public ColorData(MainWindow window, string inputColor, Dictionary<string, object> variables) : base(inputColor.Trim()) {
			_random = new Random((int)DateTime.Now.Ticks);
			_window = window;
			Variables = variables;
		}

		public override bool IsBlock => false;

		public override ParsedAction Action => ParsedAction.Color;

		public override TurtleData Compile(TurtleData previous, CancellationToken token) {
			return new TurtleData {
				Angle = previous.Angle,
				SetAngle = previous.SetAngle,
				Brush = (Brush)new BrushConverter().ConvertFromString(Arg1 == "random" ? RandColor() : Arg1),
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

		public override Task Execute(CancellationToken token) {
			if (token.IsCancellationRequested) {
				return Task.CompletedTask;
			}
			if (Arg1 == "random" || Arg1 == "rand") {
				_window.Color = RandColor();
			}
			else {
				_window.Color = Arg1;
			}
			return Task.CompletedTask;
		}

		public override ParsedData Parse(string line, StringReader reader, Dictionary<string, object> variables) {
			return this;
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