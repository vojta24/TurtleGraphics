namespace TurtleGraphics {
	public static class Helpers {
		public static double Lerp(double start, double end, double by) {
			return start * (1 - by) + end * by;
		}
	}
}
