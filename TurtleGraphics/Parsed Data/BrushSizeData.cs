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

		public BrushSizeData(IGenericExpression<double> expression, Dictionary<string, object> variables, string line) : base(variables, line) {
			_expression = expression;
		}

		public override TurtleData Compile(CancellationToken token) {
			token.ThrowIfCancellationRequested();
			UpdateVars(_expression);
			return new TurtleData {
				BrushThickness = _expression.Evaluate(),
				Action = Action,
			};
		}

		public override IList<TurtleData> CompileBlock(CancellationToken token) {
			throw new System.NotImplementedException();
		}
	}
}
