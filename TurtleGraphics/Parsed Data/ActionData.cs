using System;
using System.Threading;
using System.Threading.Tasks;

namespace TurtleGraphics {
	public class ActionData : ParsedData {
		private readonly Action _toExecute;

		public ActionData(Action action) {
			_toExecute = action;
		}

		public override Task Execute(CancellationToken token) {
			if (token.IsCancellationRequested) {
				return Task.CompletedTask;
			}
			_toExecute();
			return Task.CompletedTask;
		}
	}
}