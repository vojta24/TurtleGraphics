using System.Threading;
using System.Threading.Tasks;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class BrushSizeData : ParsedData {

		private readonly MainWindow _window;
		private readonly IGenericExpression<double> _expression;

		public const double BASE_BRUSH_SIZE = 4;

		public BrushSizeData(MainWindow window, IGenericExpression<double> expression) {
			_window = window;
			_expression = expression;
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
	}
}
