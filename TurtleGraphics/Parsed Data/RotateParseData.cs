using System;
using System.Threading;
using System.Threading.Tasks;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class RotateParseData : ParsedData {

		private readonly MainWindow _window;
		private readonly IGenericExpression<double> _expression;

		public RotateParseData(MainWindow w, IGenericExpression<double> expression, bool hardAngle) {
			_window = w;
			_expression = expression;
			SetRotation = hardAngle;
		}

		public double Angle { get; set; }
		public bool SetRotation { get; set; }

		public override Task Execute(CancellationToken token) {
			if (token.IsCancellationRequested) {
				return Task.CompletedTask;
			}

			if (double.IsNaN(Angle)) {
				_window.Rotate(0, SetRotation);
				return Task.CompletedTask;
			}

			UpdateVars(_expression);
			Angle = _expression.Evaluate();
			_window.Rotate(Angle, SetRotation);
			return Task.CompletedTask;
		}
	}
}
