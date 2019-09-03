using System;
using System.Collections.Generic;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class ParsedData<T> : ParsedData {

		public ParsedData(MainWindow win) {

		}

		public IGenericExpression<T> Exp { get; set; }

		public Func<T> Value { get; set; }

		public Action<T> Action { get; set; }
	}

	public class ParsedData {
		public IGenericExpression<object> Exp { get; set; }

		public string Line { get; set; }

		public Dictionary<string, object> Variables { get; set; }

		public Func<object> Value { get; set; }
	}
}
