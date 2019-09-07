using System;
using System.Threading;
using System.Threading.Tasks;

namespace TurtleGraphics {
	public class ForwardParseData : ParsedData {

		private readonly MainWindow _window;

		public ForwardParseData(MainWindow w) {
			_window = w;
		}

		public double Distance { get; set; }

		public override async Task Execute(CancellationToken token) {
			if (token.IsCancellationRequested) {
				return;
			}
			UpdateVars(Exp);
			Distance = Convert.ToDouble(Exp.Evaluate());
			await _window.Forward(Distance);
		}
	}
}
