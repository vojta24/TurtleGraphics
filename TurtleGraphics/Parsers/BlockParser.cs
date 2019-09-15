﻿using System.Collections.Generic;
using System.IO;

namespace TurtleGraphics {
	public static class BlockParser {
		public static List<string> ParseBlock(StringReader reader) {
			List<string> ret = new List<string>();

			string next = reader.ReadLine();
			int openBarckets = 1;

			while (next != null){
				ret.Add(next);
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
				next = reader.ReadLine();
			}

			if (openBarckets != 0) {
				throw new ParsingException("Missing closing bracket!") { LineText = "" };
			}

			return ret;
		}
	}
}
