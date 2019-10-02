using System.Collections.Generic;
using System.Threading;

namespace TurtleGraphics {
	public class RestorePositionData : ParsedData {

		public RestorePositionData(string[] args, Dictionary<string, object> variables, string line) : base(variables, line) {
			Parameters = args;
			if (Arg1 != null) {
				IsPop = bool.Parse(Arg1);
			}
		}

		public bool IsPop { get; set; }

		public override bool IsBlock => false;

		public override string Line { get; set; }

		public override ParsedAction Action => ParsedAction.RestorePos;

		public override TurtleData Compile(TurtleData previous, CancellationToken token) {
			return new TurtleData {
				Action = Action,
				PopPosition = IsPop
			};
		}

		public override IList<TurtleData> CompileBlock(TurtleData previous, CancellationToken token) {
			throw new System.NotImplementedException();
		}
	}
}
