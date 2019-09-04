using System;
using System.Text;
using System.Threading.Tasks;

namespace TurtleGraphics {
	internal class ColorData : ParsedData {
		private readonly char[] colorVals = new[] { '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
		private static Random r = new Random();
		private string color;
		private MainWindow w;

		public ColorData(MainWindow win, string v) {

			color = v;
			w = win;
		}

		public override Task Execute() {
			if (color == "random") {
				w.Color = RandColor();
			}
			else {
				w.Color = color;
			}
			return Task.CompletedTask;
		}

		private string RandColor() {
			StringBuilder builder = new StringBuilder("#");

			for (int i = 0; i < 6; i++) {
				char c = colorVals[r.Next(0, colorVals.Length)];
				builder.Append(c);
			}
			return builder.ToString();
		}
	}
}