using System.Threading.Tasks;

namespace TurtleGraphics {
	internal class StringData : ParsedData {
		private string color;
		private MainWindow w;

		public StringData(MainWindow win, string v) {

			color = v;
			w = win;
		}

		public override Task Execute() {
			w.Color = color;
			return Task.CompletedTask;
		}
	}
}