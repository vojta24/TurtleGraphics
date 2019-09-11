using System.Collections.Generic;

namespace TurtleGraphics {
	public static class Extensions {
		public static Dictionary<T, K> Copy<T, K>(this Dictionary<T, K> dict) {
			return new Dictionary<T, K>(dict);
		}
	}
}
