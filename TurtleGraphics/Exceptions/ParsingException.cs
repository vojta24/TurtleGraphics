using System;

namespace TurtleGraphics {
	public sealed class ParsingException : Exception {
		public ParsingException(string message) : base(message) {
			//LineIndex = lineIndex;
		}

		public int LineIndex { get; set; }

		public string LineText { get; set; }

		public ParsingException(string message, Exception innerException) : base(message, innerException) {
			//LineIndex = lineIndex;
		}
	}
}
