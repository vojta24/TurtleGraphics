using System;
using System.Collections.Generic;

namespace TurtleGraphics {
	public class InteliCommandsHandler {

		private static Dictionary<string, string> _inteliCommands = new Dictionary<string, string> {
			{ "for", " (int i = 0; i < ; i++) {" + Environment.NewLine + Environment.NewLine + "}" },
			{ "if", " () {" + Environment.NewLine + Environment.NewLine + "}" },
		};

		private static Dictionary<string, int> _inteliCommandsIndexes = new Dictionary<string, int> {
			{ "for", 17  },
			{ "if", 2 },
		};

		public static string GetIntliCommand(string value) {
			foreach (string key in _inteliCommands.Keys) {
				if (value.EndsWith(key)) {
					return value + _inteliCommands[key];
				}
			}
			return value;
		}

		public static int GetIndexForCaret(string value) {
			foreach (string key in _inteliCommands.Keys) {
				if (value.EndsWith(key)) {
					return value.Length + _inteliCommandsIndexes[key];
				}
			}
			throw new NotImplementedException();
		}
	}
}