using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class ForLoopData : ParsedData {
		public int From { get; set; }
		public int To { get; set; }
		public string LoopVariable { get; set; }
		public Dictionary<string, object> InheritedVariables { get; set; }
		public List<string> Lines { get; set; }

		public override async Task Execute() {
			List<ParsedData> loopContents = CompileLoop();

			for (int i = From; i < To; i++) {
				foreach (ParsedData data in loopContents) {
					if (data.Exp != null) {
						data.Exp.Context.Variables[LoopVariable] = i;
					}
					await data.Execute();
				}
			}
		}

		private List<ParsedData> CompileLoop() {
			ExpressionContext c = new ExpressionContext();
			c.Variables.Add(LoopVariable, From);
			foreach (string key in InheritedVariables.Keys) {
				c.Variables.Add(key, InheritedVariables[key]);
			}

			List<ParsedData> singleIteration = new List<ParsedData>();

			foreach (string line in Lines) {
				Queue<ParsedData> data = CommandParser.Parse(string.Join(Environment.NewLine,Lines), CommandParser.win, new Dictionary<string, object> { { LoopVariable, 0 } }).Result;
				singleIteration.AddRange(data);
			}
			return singleIteration;
		}
	}
}
