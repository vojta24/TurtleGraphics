﻿using System.Collections.Generic;
using System.Threading;
using Flee.PublicTypes;

namespace TurtleGraphics {

	public abstract class ParsedData {


		public ParsedData(VariableStore variables, string originalLine, int index, params string[] parameters) {
			Parameters = parameters;
			Variables = variables;
			Line = originalLine;
			LineIndex = index;
		}

		public abstract bool IsBlock { get; }

		public abstract string Line { get; set; }
		public int LineIndex { get; }

		public VariableStore Variables { get; set; }

		public string[] Parameters { get; set; }

		public string Arg1 => Parameters.Length > 0 ? Parameters[0] : null;
		public string Arg2 => Parameters.Length > 1 ? Parameters[1] : null;
		public string Arg3 => Parameters.Length > 2 ? Parameters[2] : null;
		public string Arg4 => Parameters.Length > 3 ? Parameters[3] : null;

		public abstract ParsedAction Action { get; }

		public abstract TurtleData Compile(CancellationToken token);


		public abstract IList<TurtleData> CompileBlock(CancellationToken token, int indent);
	}
}
