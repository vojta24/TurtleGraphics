using System.Collections.Generic;
using System.Threading;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class BrushSizeData : ParsedData {

		private readonly IGenericExpression<double> _expression;

		public const double BASE_BRUSH_SIZE = 4;

		public override bool IsBlock => false;

		public override ParsedAction Action => ParsedAction.Thickness;

		public override string Line { get; set; }

		public BrushSizeData(IGenericExpression<double> expression, VariableStore variables, string line, int lineIndex) : base(variables, line, lineIndex) {
			_expression = expression;
		}

		public override TurtleData Compile(CancellationToken token) {
			token.ThrowIfCancellationRequested();
			Variables.Update(_expression, LineIndex);
			return new TurtleData {
				BrushThickness = _expression.Evaluate(),
				Action = Action,
			};
		}

		public override IList<TurtleData> CompileBlock(CancellationToken token, int indent) {
			throw new System.NotImplementedException();
		}
	}
}
