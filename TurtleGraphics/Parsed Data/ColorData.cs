using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Media;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class ColorData : ParsedData {

		private readonly char[] HEX = new[] { '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
		private readonly Random _random;

		public ColorData(string[] args, Dictionary<string, object> variables, string line) : base(variables, line, args) {
			_random = new Random((int)DateTime.Now.Ticks);
		}

		public override bool IsBlock => false;

		public override ParsedAction Action => ParsedAction.Color;

		public override string Line { get; set; }

		public override TurtleData Compile(TurtleData previous, CancellationToken token) {
			token.ThrowIfCancellationRequested();
			Brush brush;

			if (Parameters.Length == 1) {
				string colorData = Arg1;

				foreach (string key in Variables.Keys) {
					if (colorData.Contains(key) && colorData != "random") {
						colorData = colorData.Replace(key, Variables[key].ToString());
					}
				}
				try {
					brush = (Brush)new BrushConverter().ConvertFromString(Arg1 == "random" ? RandColor() : Arg1);
				}
				catch (FormatException e) {
					throw new ParsingException("Invalid token for 'Color'", Line, e);
				}
			}
			else if (Parameters.Length == 3) {
				ExpressionContext c = FleeHelper.GetExpression(Variables);
				try {
					byte r = Convert.ToByte(c.CompileGeneric<double>(Arg1).Evaluate());
					byte g = Convert.ToByte(c.CompileGeneric<double>(Arg2).Evaluate());
					byte b = Convert.ToByte(c.CompileGeneric<double>(Arg3).Evaluate());
					brush = new SolidColorBrush(new Color() { A = byte.MaxValue, R = r, G = g, B = b });
				}
				catch (Exception e) {
					throw new ParsingException("Invalid value, expected " + typeof(byte).Name, Line, e);
				}
			}
			else if (Parameters.Length == 4) {
				ExpressionContext c = FleeHelper.GetExpression(Variables);
				try {
					byte a = Convert.ToByte(c.CompileGeneric<double>(Arg1).Evaluate());
					byte r = Convert.ToByte(c.CompileGeneric<double>(Arg2).Evaluate());
					byte g = Convert.ToByte(c.CompileGeneric<double>(Arg3).Evaluate());
					byte b = Convert.ToByte(c.CompileGeneric<double>(Arg4).Evaluate());

					brush = new SolidColorBrush(new Color() { A = a, R = r, G = g, B = b });
				}
				catch (Exception e) {
					throw new ParsingException("Invalid value, expected " + typeof(byte).Name, Line, e);
				}
			}
			else {
				throw new ParsingException("Non-existent overload for function!", Line);
			}

			brush.Freeze();

			return new TurtleData {
				Brush = brush,
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