using System;

namespace TurtleGraphics {
	public sealed class ParsingException : Exception {
		public ParsingException(string message, string lineText) : base(message) {
			LineText = lineText;
		}

		public int LineIndex { get; set; }

		public string LineText { get; set; }

		public ParsingException(string message, string lineText, Exception innerException) : base(message, innerException) {
			LineText = lineText;
		}
	}
}
