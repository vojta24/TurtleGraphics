using System;
using System.Threading.Tasks;

namespace TurtleGraphics {
	internal class ActionData : ParsedData {
		private readonly Action _toExecute;

		public ActionData(Action action) {
			_toExecute = action;
		}

		public override Task Execute() {
			_toExecute();
			return Task.CompletedTask;
		}
	}
}