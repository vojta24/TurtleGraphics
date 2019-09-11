using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class ForwardParseData : ParsedData {

		private readonly IGenericExpression<double> _expression;

		public ForwardParseData(IGenericExpression<double> expression, Dictionary<string, object> variables) {
			_expression = expression;
			Variables = variables;
		}

		public double Distance { get; set; }

		public override bool IsBlock => false;

		public override ParsedAction Action => ParsedAction.Forward;

		public override TurtleData Compile(TurtleData previous, CancellationToken token) {
			UpdateVars(_expression);
			Distance = _expression.Evaluate();
			return new TurtleData {
				Angle = previous.Angle,
				SetAngle = previous.SetAngle,
				Distance = Distance,
				Brush = previous.Brush,
				BrushThickness = previous.BrushThickness,
				MoveTo = new Point(previous.MoveTo.X + Math.Cos(previous.Angle) * Distance, previous.MoveTo.Y + Math.Sin(previous.Angle) * Distance),
				PenDown = previous.PenDown,
				Jump = false,
				Action = Action,
			};
		}

		public override IList<TurtleData> CompileBlock(TurtleData previous, CancellationToken token) {
			throw new NotImplementedException();
		}
	}
}
