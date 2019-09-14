using System.Collections.Generic;

namespace TurtleGraphics.Parsers {
	public static class ArgParser {
		public static string[] Parse(string args) {
			List<string> parsed = new List<string>();

			bool isString = false;
			bool isChar = false;
			int innerLevel = 0;

			int start = 0;

			for (int i = 0; i < args.Length; i++) {
				if (args[i] == '(') {
					innerLevel++;
				}
				if (args[i] == ')') {
					innerLevel--;
				}
				if (args[i] == '\'') {
					isChar ^= true;
				}
				if (args[i] == '\"') {
					isString ^= true;
				}

				if (!isString && !isChar && innerLevel == 0 && args[i] == ',') {
					parsed.Add(args.Substring(start, i));
					parsed.AddRange(Parse(args.Substring(i + 1)));
					break;
				}
			}
			if(parsed.Count == 0) {
				parsed.Add(args);
			}
			return parsed.ToArray();
		}
	}
}
