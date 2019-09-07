using System.Collections.Generic;
using Flee.PublicTypes;

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


		public static void AddRange(this VariableCollection a, Dictionary<string, object> b) {
			foreach (var item in b) {
				a[item.Key] = item.Value;
			}
		}
	}
}
