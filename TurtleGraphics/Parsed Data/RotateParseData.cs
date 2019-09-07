using System;
using System.Threading;
using System.Threading.Tasks;

namespace TurtleGraphics {
	public class RotateParseData : ParsedData {

		private readonly MainWindow _window;

		public RotateParseData(MainWindow w, double val, bool hardAngle) {
			_window = w;
			Angle = val;
			SetRotation = hardAngle;
		}

		public double Angle { get; set; }
		public bool SetRotation { get; set; }

		public override Task Execute(CancellationToken token) {
			if (token.IsCancellationRequested) {
				return Task.CompletedTask;
			}
			if (double.IsNaN(Angle)) {
				_window.Rotate(Angle, SetRotation);
				return Task.CompletedTask;
			}
			UpdateVars(Exp);
			Angle = Convert.ToDouble(Exp.Evaluate());
			_window.Rotate(Angle, SetRotation);
			return Task.CompletedTask;
		}
	}
}
