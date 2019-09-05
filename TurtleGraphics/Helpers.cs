using System.Collections.Generic;

namespace TurtleGraphics {
	public static class Helpers {
		public static double Lerp(double start, double end, double by) {
			return start * (1 - by) + end * by;
		}

		public static Dictionary<string, object> Join(Dictionary<string, object> a, Dictionary<string, object> b) {
			Dictionary<string, object> join = new Dictionary<string, object>(a);
			foreach (var item in b) {
				join[item.Key] = item.Value;
			}
			return join;
		}
	}
}
