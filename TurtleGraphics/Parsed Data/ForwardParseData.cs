using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class ForwardParseData : ParsedData {

		private readonly IGenericExpression<double> _expression;

		public ForwardParseData(IGenericExpression<double> expression, Dictionary<string, object> variables, string line) : base(variables, line) {
			_expression = expression;
		}

		public double Distance { get; set; }

		public override bool IsBlock => false;

		public override ParsedAction Action => ParsedAction.Forward;

		public override string Line { get; set; }

		public override TurtleData Compile(TurtleData previous, CancellationToken token) {
			token.ThrowIfCancellationRequested();
			UpdateVars(_expression);
			Distance = _expression.Evaluate();
			return new TurtleData {
				MoveTo = new Point(previous.MoveTo.X + Math.Cos(previous.Angle) * Distance, previous.MoveTo.Y + Math.Sin(previous.Angle) * Distance),
				Distance = Distance,
				Action = Action,
			};
		}

		public override IList<TurtleData> CompileBlock(TurtleData previous, CancellationToken token) {
			throw new NotImplementedException();
		}
	}
}
