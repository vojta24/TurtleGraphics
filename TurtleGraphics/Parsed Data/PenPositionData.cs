using System;
using System.Collections.Generic;
using System.Threading;

namespace TurtleGraphics {
	public class PenPositionData : ParsedData {

		public PenPositionData(bool state, Dictionary<string, object> variables) {
			PenState = state;
			Variables = variables;
		}

		public bool PenState { get; set; }

		public override bool IsBlock => false;

		public override ParsedAction Action => ParsedAction.PenState;

		public override TurtleData Compile(TurtleData previous, CancellationToken token) {
			token.ThrowIfCancellationRequested();
			return new TurtleData {
				Angle = previous.Angle,
				SetAngle = previous.SetAngle,
				Brush = previous.Brush,
				BrushThickness = previous.BrushThickness,
				MoveTo = previous.MoveTo,
				PenDown = PenState,
				Jump = false,
				Action = Action,
			};
		}

		public override IList<TurtleData> CompileBlock(TurtleData previous, CancellationToken token) {
			throw new NotImplementedException();
		}
	}
}