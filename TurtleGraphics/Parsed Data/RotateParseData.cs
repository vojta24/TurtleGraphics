using System;
using System.Collections.Generic;
using System.Threading;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class RotateParseData : ParsedData {

		private readonly IGenericExpression<double> _expression;

		public RotateParseData(IGenericExpression<double> expression, FunctionCallInfo info, Dictionary<string, object> variables, string line) : base(variables, line) {
			_expression = expression;
			string exceptionMessage = "Invalid arguments for rotation";
			try {
				if (info.Arguments.Length == 2) {
					SetRotation = bool.Parse(info.Arguments[1]);
				}
			}
			catch (Exception e) {
				throw new ParsingException(exceptionMessage, e) { LineText = line };
			}
		}

		public double Angle { get; set; } = double.NaN;
		public bool SetRotation { get; set; }

		public override bool IsBlock => false;

		public override ParsedAction Action => ParsedAction.Rotate;

		public override string Line { get; set; }

		public override TurtleData Compile(TurtleData previous, CancellationToken token) {
			token.ThrowIfCancellationRequested();
			UpdateVars(_expression);
			return new TurtleData {
				Angle = _expression == null ? 0 : _expression.Evaluate(),
				SetAngle = SetRotation,
				Action = Action,
			};
		}

		public override IList<TurtleData> CompileBlock(TurtleData previous, CancellationToken token) {
			throw new NotImplementedException();
		}
	}
}
