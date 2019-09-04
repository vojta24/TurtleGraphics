using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flee.PublicTypes;

namespace TurtleGraphics {

	public class ParsedData {
		public virtual IDynamicExpression Exp { get; set; }

		public string Line { get; set; }

		public Dictionary<string,object> Variables { get; set; }

		public virtual Task Execute() => throw new NotImplementedException();

		protected void UpdateVars(IDynamicExpression exp) {
			foreach (var item in Variables) {
				exp.Context.Variables[item.Key] = item.Value;
			}
		}
	}
}
