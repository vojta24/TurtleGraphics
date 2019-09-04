using System;
using System.Threading.Tasks;

namespace TurtleGraphics {
	internal class ActionData : ParsedData {
		private Func<bool> p;

		public ActionData(Func<bool> p) {
			this.p = p;
		}

		public override Task Execute() {
			p();
			return Task.CompletedTask;
		}
	}
}