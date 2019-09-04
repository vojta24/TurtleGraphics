using System;
using System.Threading.Tasks;

namespace TurtleGraphics {
	public class ForwardParseData : ParsedData {

		private readonly MainWindow _window;

		public ForwardParseData(MainWindow w) {
			_window = w;
		}

		public double Distance { get; set; }

		public override async Task Execute() {
			UpdateVars(Exp);
			Distance = Convert.ToDouble(Exp.Evaluate());
			await _window.Forward(Distance);
		}
	}
}
