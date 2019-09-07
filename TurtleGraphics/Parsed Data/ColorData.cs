using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TurtleGraphics {
	public class ColorData : ParsedData {

		private readonly char[] HEX = new[] { '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
		private readonly Random _random;
		private readonly MainWindow _window;

		public ColorData(MainWindow window, string inputColor) : base(inputColor.Trim()) {
			_random = new Random((int)DateTime.Now.Ticks);
			_window = window;
		}

		public override Task Execute(CancellationToken token) {
			if (token.IsCancellationRequested) {
				return Task.CompletedTask;
			}
			if (Arg1 == "random" || Arg1 == "rand") {
				_window.Color = RandColor();
			}
			else {
				_window.Color = Arg1;
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