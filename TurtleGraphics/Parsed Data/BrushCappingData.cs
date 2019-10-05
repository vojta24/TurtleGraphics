using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Media;

namespace TurtleGraphics {
	public class BrushCappingData : ParsedData {
		public BrushCappingData(string[] args, VariableStore variables, string line) : base(variables, line, args) { }

		public override bool IsBlock => false;

		public override string Line { get; set; }

		public override ParsedAction Action => ParsedAction.Capping;

		public override TurtleData Compile(CancellationToken token) {
			token.ThrowIfCancellationRequested();

			return new TurtleData {
				Action = Action,
				LineCap = (PenLineCap)Enum.Parse(typeof(PenLineCap), Arg1),
			};
		}

		public override IList<TurtleData> CompileBlock(CancellationToken token, int indent) {
			throw new NotImplementedException();
		}
	}
}
