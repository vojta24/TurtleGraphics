using System;
using System.Collections.Generic;
using System.Threading;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class RotateParseData : ParsedData {

		private readonly IGenericExpression<double> _expression;

		public RotateParseData(IGenericExpression<double> expression, FunctionCallInfo info, Dictionary<string, object> variables) {
			_expression = expression;
			Variables = variables;

			if (info.Arguments.Length == 2) {
				bool.TryParse(info.Arguments[1], out _setRotation);
			}
		}

		public double Angle { get; set; } = double.NaN;
		private bool _setRotation;
		public bool SetRotation { get => _setRotation; set => _setRotation = value; }

		public override bool IsBlock => false;

		public override ParsedAction Action => ParsedAction.Rotate;

		public override TurtleData Compile(TurtleData previous, CancellationToken token) {
			token.ThrowIfCancellationRequested();
			UpdateVars(_expression);
			return new TurtleData {
				Angle = _expression == null ? 0 : _expression.Evaluate(),
				SetAngle = SetRotation,
				Brush = previous.Brush,
				BrushThickness = previous.BrushThickness,
				PenDown = previous.PenDown,
				MoveTo = previous.MoveTo,
				Jump = false,
				Action = Action,
			};
		}

		public override IList<TurtleData> CompileBlock(TurtleData previous, CancellationToken token) {
			throw new NotImplementedException();
		}
	}
}
