using System;
using System.Text;
using System.Threading.Tasks;

namespace TurtleGraphics {
	internal class ColorData : ParsedData {

		private readonly char[] HEX = new[] { '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
		private readonly Random _random;
		private readonly string _color;
		private readonly MainWindow _window;

		public ColorData(MainWindow window, string inputColor) {
			_random = new Random((int)DateTime.Now.Ticks);
			_color = inputColor.Trim().ToLower();
			_window = window;
		}

		public override Task Execute() {
			if (_color == "random" || _color == "rand") {
				_window.Color = RandColor();
			}
			else {
				_window.Color = _color;
			}
			return Task.CompletedTask;
		}

		private string RandColor() {
			StringBuilder builder = new StringBuilder("#");

			for (int i = 0; i < 6; i++) {
				char c = HEX[_random.Next(0, HEX.Length)];
				builder.Append(c);
			}
			return builder.ToString();
		}
	}
}