using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class MoveData : ParsedData {

		private readonly IGenericExpression<double> x;
		private readonly IGenericExpression<double> y;

		public MoveData(string[] args, Dictionary<string, object> variables, string line) : base(variables, line, args) {
			ExpressionContext expression = FleeHelper.GetExpression(variables);
			string exceptionMessage = "";
			try {
				exceptionMessage = "Invalid expression for X coordinate!";
				x = expression.CompileGeneric<double>(args[0]);
				exceptionMessage = "Invalid expression for Y coordinate!";
				y = expression.CompileGeneric<double>(args[1]);
			}
			catch (Exception e) {
				throw new ParsingException(exceptionMessage, e) { LineText = line };
			}
		}

		public override bool IsBlock => false;

		public override ParsedAction Action => ParsedAction.MoveTo;

		public override string Line { get; set; }

		public override TurtleData Compile(TurtleData previous, CancellationToken token) {
			token.ThrowIfCancellationRequested();
			UpdateVars(x);
			UpdateVars(y);

			return new TurtleData {
				Angle = previous.Angle,
				SetAngle = previous.SetAngle,
				Brush = previous.Brush,
				BrushThickness = previous.BrushThickness,
				PenDown = previous.PenDown,
				MoveTo = new Point(x.Evaluate(), y.Evaluate()),
				Jump = true,
				Action = Action,
			};
		}

		public override IList<TurtleData> CompileBlock(TurtleData previous, CancellationToken token) {
			throw new NotImplementedException();
		}
	}
}