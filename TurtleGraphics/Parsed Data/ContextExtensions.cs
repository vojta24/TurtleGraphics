using System;

namespace TurtleGraphics {
	public static class ContextExtensions {

		private static readonly Random random = new Random();

		public static double AsRad(double degrees) {
			return Math.PI * degrees / 180.0;
		}

		public static double AsDeg(double rad) {
			return rad * (180.0 / Math.PI);
		}

		public static double RandX() {
			return random.NextDouble() * MainWindow.Instance.DrawWidth;
		}

		public static double RandY() {
			return random.NextDouble() * MainWindow.Instance.DrawHeight;
		}
	}
}
