using System;

namespace TurtleGraphics {
	public static class ContextExtensions {

		private static MainWindow window;
		private static readonly Random random = new Random();

		public static double AsRad(double degrees) {
			return Math.PI * degrees / 180.0;
		}

		public static double AsDeg(double rad) {
			return rad * (180.0 / Math.PI);
		}

		public static double RandX() {
			return random.NextDouble() * window.DrawWidth;
		}

		public static double RandY() {
			return random.NextDouble() * window.DrawHeight;
		}

		internal static void SetWindow(MainWindow mainWindow) {
			window = mainWindow;
		}
	}
}
