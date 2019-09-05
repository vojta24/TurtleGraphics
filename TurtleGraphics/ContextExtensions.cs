using System;

namespace TurtleGraphics {
	public static class ContextExtensions {
		public static double AsRad(double degrees) {
			return Math.PI * degrees / 180.0;
		}

		public static double AsDeg(double rad) {
			return rad * (180.0 / Math.PI);
		}
	}
}
