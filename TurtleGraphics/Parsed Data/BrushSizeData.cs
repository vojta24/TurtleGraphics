using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class BrushSizeData : ParsedData {

		private readonly MainWindow _window;
		private readonly IGenericExpression<double> _expression;

		public const double BASE_BRUSH_SIZE = 4;

		public override bool IsBlock => false;

		public override ParsedAction Action => ParsedAction.Thickness;

		public BrushSizeData(MainWindow window, IGenericExpression<double> expression, Dictionary<string, object> variables) {
			_window = window;
			_expression = expression;
			Variables = variables;
		}

		public override Task Execute(CancellationToken token) {
			if (token.IsCancellationRequested) {
				return Task.CompletedTask;
			}

			UpdateVars(_expression);
			_window.BrushSize = _expression.Evaluate();
			double scale = _window.BrushSize / BASE_BRUSH_SIZE;
			_window.TurtleScale.ScaleX = _window.TurtleScale.ScaleY = scale;
			return Task.CompletedTask;
		}

		public override ParsedData Parse(string line, StringReader reader, Dictionary<string, object> variables) {
			return this;
		}

		public override TurtleData Compile(TurtleData previous, CancellationToken token) {
			UpdateVars(_expression);
			return new TurtleData {
				Angle = previous.Angle,
				SetAngle = previous.SetAngle,
				Brush = previous.Brush,
				BrushThickness = _expression.Evaluate(),
				MoveTo = previous.MoveTo,
				PenDown = previous.PenDown,
				Jump = false,
				Action = Action,
			};
		}

		public override IList<TurtleData> CompileBlock(TurtleData previous, CancellationToken token) {
			throw new System.NotImplementedException();
		}
	}
}
