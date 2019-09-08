using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Flee.PublicTypes;

namespace TurtleGraphics {

	public abstract class ParsedData {

		public ParsedData(params string[] parameters) {
			Parameters = parameters;
		}

		public string Line { get; set; }

		public Dictionary<string, object> Variables { get; set; }

		public string[] Parameters { get; set; }

		public string Arg1 => Parameters[0];
		public string Arg2 => Parameters[1];

		public abstract Task Execute(CancellationToken token);

		public abstract ParsedData Parse(string line, StringReader reader, Dictionary<string, object> variables);

		protected void UpdateVars(IDynamicExpression exp) {
			foreach (var item in Variables) {
				exp.Context.Variables[item.Key] = item.Value;
			}
		}

		protected void UpdateVars<T>(IGenericExpression<T> exp) {
			foreach (var item in Variables) {
				exp.Context.Variables[item.Key] = item.Value;
			}
		}
	}
}
