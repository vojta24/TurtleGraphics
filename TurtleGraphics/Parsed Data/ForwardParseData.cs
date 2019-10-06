using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class ForwardParseData : ParsedData {

		private readonly IGenericExpression<double> _expression;

		public ForwardParseData(IGenericExpression<double> expression, VariableStore variables, string line, int lineIndex) : base(variables, line, lineIndex) {
			_expression = expression;
		}

		public double Distance { get; set; }

		public override bool IsBlock => false;

		public override ParsedAction Action => ParsedAction.Forward;

		public override string Line { get; set; }

		public override TurtleData Compile(CancellationToken token) {
			token.ThrowIfCancellationRequested();
			Variables.Update(_expression, LineIndex);
			Distance = _expression.Evaluate();
			return new TurtleData {
				Distance = Distance,
				Action = Action,
			};
		}

		public override IList<TurtleData> CompileBlock(CancellationToken token, int indent) {
			throw new NotImplementedException();
		}
	}
}
