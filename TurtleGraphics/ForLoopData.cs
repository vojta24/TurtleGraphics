using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TurtleGraphics {
	public class ForLoopData {
		public int From { get; set; }
		public int To { get; set; }
		public string Var { get; set; }
		public Queue<Func<Task>> Queue { get; set; }
	}
}
