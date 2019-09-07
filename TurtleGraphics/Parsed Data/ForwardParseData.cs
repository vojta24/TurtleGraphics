using System;
using System.Threading;
using System.Threading.Tasks;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class ForwardParseData : ParsedData {

		private readonly MainWindow _window;
		private readonly IGenericExpression<double> _expression;

		public ForwardParseData(MainWindow w, IGenericExpression<double> expression) {
			_window = w;
			_expression = expression;
		}

		public double Distance { get; set; }

		public override async Task Execute(CancellationToken token) {
			if (token.IsCancellationRequested) {
				return;
			}
			UpdateVars(_expression);
			Distance = _expression.Evaluate();
			await _window.Forward(Distance);
		}
	}
}
