using System;
using System.Threading.Tasks;
using Flee.PublicTypes;

namespace TurtleGraphics {

	public class ParsedData {
		public virtual IDynamicExpression Exp { get; set; }

		public string Line { get; set; }

		public virtual Task Execute() => throw new NotImplementedException();
	}
}
