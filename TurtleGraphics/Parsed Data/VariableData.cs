using System.Collections.Generic;
using System.Threading;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class VariableData : ParsedData {
		public override bool IsBlock => false;

		public override string Line { get; set; }

		public override ParsedAction Action => ParsedAction.VariableChange;

		public string VariableName { get; }
		public IDynamicExpression Value { get; }

		public bool IsDefinition { get; set; }

		public VariableData(string varName, IDynamicExpression value, bool isDef, VariableStore variables, string line, int lineIndex) : base(variables, line, lineIndex) {
			VariableName = varName;
			Value = value;
			IsDefinition = isDef;
		}

		public override TurtleData Compile(CancellationToken token) {
			return TurtleData.NoAction;
		}

		public override IList<TurtleData> CompileBlock(CancellationToken token, int indent) {
			throw new System.NotImplementedException();
		}
	}
}
