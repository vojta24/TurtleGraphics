using System;
using System.Collections.Generic;
using System.Threading;

namespace TurtleGraphics {
	public class PenPositionData : ParsedData {

		public PenPositionData(bool state, Dictionary<string, object> variables, string line) : base(variables, line) {
			PenState = state;
		}

		public bool PenState { get; set; }

		public override bool IsBlock => false;

		public override ParsedAction Action => ParsedAction.PenState;

		public override string Line { get; set; }

		public override TurtleData Compile(TurtleData previous, CancellationToken token) {
			token.ThrowIfCancellationRequested();
			return new TurtleData {
				PenDown = PenState,
				Action = Action,
			};
		}

		public override IList<TurtleData> CompileBlock(TurtleData previous, CancellationToken token) {
			throw new NotImplementedException();
		}
	}
}