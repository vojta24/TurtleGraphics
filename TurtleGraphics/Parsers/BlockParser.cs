using System.Collections.Generic;
using System.IO;

namespace TurtleGraphics {
	public static class BlockParser {
		public static List<string> ParseBlock(StringReader reader) {
			List<string> ret = new List<string>();

			string next = reader.ReadLine();
			int openBarckets = 1;

			if (next.Contains("{")) {
				openBarckets++;
			}
			if (next.Contains("}")) {
				openBarckets--;
				if (openBarckets == 0) {
					ret.Add(next);
					return ret;
				}
			}
			do {
				ret.Add(next);
				next = reader.ReadLine();
				if (next.Contains("{")) {
					openBarckets++;
				}
				if (next.Trim() == "}") {
					openBarckets--;
					if (openBarckets == 0) {
						ret.Add(next);
						break;
					}
				}
			}
			while (next != null);
			return ret;
		}
	}
}
