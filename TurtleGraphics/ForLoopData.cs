using System.Collections.Generic;
using System.Threading.Tasks;

namespace TurtleGraphics {
	public class ForLoopData : ParsedData {
		public int From { get; set; }
		public int To { get; set; }
		public string Var { get; set; }
		public Queue<ParsedData> Queue { get; set; }

		public override ParsedData Clone() {
			return new ForLoopData {
				Variables = new Dictionary<string, object>(this.Variables),
				Line = this.Line,
				Value = this.Value,
				Exp = this.Exp,
				From = this.From,
				To = this.To,
				Var = this.Var,
				Queue = new Queue<ParsedData>(this.Queue)
			};
		}


		public override Task Execute() {
			return Task.Run(async () => {
				while(Queue.Count > 0) { 
					ParsedData data = Queue.Dequeue();
					await data.Execute();
				}
			});
		}
	}
}
