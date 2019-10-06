using System;
using System.Collections.Generic;
using System.Threading;

namespace TurtleGraphics {
	public class PenPositionData : ParsedData {

		public PenPositionData(bool state, VariableStore variables, string line, int lineIndex) : base(variables, line, lineIndex) {
			PenState = state;
		}

		public bool PenState { get; set; }

		public override bool IsBlock => false;

		public override ParsedAction Action => ParsedAction.PenState;

		public override string Line { get; set; }

		public override TurtleData Compile(CancellationToken token) {
			token.ThrowIfCancellationRequested();
			return new TurtleData {
				PenDown = PenState,
				Action = Action,
			};
		}

		public override IList<TurtleData> CompileBlock(CancellationToken token, int indent) {
			throw new NotImplementedException();
		}
	}
}