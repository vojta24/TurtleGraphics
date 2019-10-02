using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Media;

namespace TurtleGraphics {
	public class BrushCappingData : ParsedData {
		public BrushCappingData(string[] args, Dictionary<string, object> variables, string line) : base(variables, line, args) { }

		public override bool IsBlock => false;

		public override string Line { get; set; }

		public override ParsedAction Action => ParsedAction.Capping;

		public override TurtleData Compile(TurtleData previous, CancellationToken token) {
			token.ThrowIfCancellationRequested();

			return new TurtleData {
				Action = Action,
				LineCap = (PenLineCap)Enum.Parse(typeof(PenLineCap), Arg1),
			};
		}

		public override IList<TurtleData> CompileBlock(TurtleData previous, CancellationToken token) {
			throw new NotImplementedException();
		}
	}
}
