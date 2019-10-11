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
			if(info.Arguments.Length > 2) {
				throw new ParsingException("Extra agruments supplied, maximum of 2 allowed for this function." ,line);
			}
			if (info.Arguments.Length == 2) {
				try {
					SetRotation = bool.Parse(info.GetArg(1, line));
				}
				catch (Exception e) {
					throw new ParsingException(exceptionMessage, line, e);
				}
			}
		}

		public double Angle { get; set; } = double.NaN;
		public bool SetRotation { get; set; }

		public override bool IsBlock => false;

		public override ParsedAction Action => ParsedAction.Rotate;

		public override string Line { get; set; }

		public override TurtleData Compile(CancellationToken token) {
			token.ThrowIfCancellationRequested();
			UpdateVars(_expression);
			return new TurtleData {
				Angle = _expression == null ? 0 : _expression.Evaluate(),
				SetAngle = SetRotation,
				Action = Action,
			};
		}

		public override IList<TurtleData> CompileBlock(CancellationToken token) {
			throw new NotImplementedException();
		}
	}
}
