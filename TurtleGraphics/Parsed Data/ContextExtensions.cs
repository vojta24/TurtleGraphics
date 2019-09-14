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

		public static double Rand(double from, double to) {
			return random.Next((int)from, (int)to - 1) + random.NextDouble();
		}

		public static int Map(double value, double min, double max, double newMin, double newMax) {
			double slope = (newMax - newMin) / (max - min);
			return (int)(newMin + slope * (value - min));
		}
	}
}
