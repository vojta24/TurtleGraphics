using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flee.PublicTypes;

namespace TurtleGraphics {

	public class ParsedData {

		public ParsedData(params string[] parameters) {
			Parameters = parameters;
		}

		public virtual IDynamicExpression Exp { get; set; }

		public string Line { get; set; }

		public Dictionary<string,object> Variables { get; set; }

		public string[] Parameters { get; set; }

		public string Arg1 => Parameters[0];

		public virtual Task Execute() => throw new NotImplementedException();

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
