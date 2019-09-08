using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class ConditionalData : ParsedData {

		public IGenericExpression<bool> IfCondition { get; set; }
		public Queue<ParsedData> IfData { get; set; }
		public Queue<ParsedData> ElseData { get; set; } = null;
		public IList<(IGenericExpression<bool>, Queue<ParsedData>)> ElseIfs { get; set; }
		public bool IsModifiable { get; set; } = true;

		public ConditionalData(string line,
			IGenericExpression<bool> ifCondition, Queue<ParsedData> data,
			IList<(IGenericExpression<bool>, Queue<ParsedData>)> elseIfs = null) : base(line) {
			IfCondition = ifCondition;
			IfData = data;
			ElseIfs = elseIfs;
		}

		public override async Task Execute(CancellationToken token) {
			if (token.IsCancellationRequested) {
				return;
			}
			UpdateVars(IfCondition);
			if (IfCondition.Evaluate()) {
				await ExecuteQueue(IfData, token);
			}
			else {
				if (ElseIfs != null) {
					foreach ((IGenericExpression<bool> exp, Queue<ParsedData> data) in ElseIfs) {
						UpdateVars(exp);
						if (exp.Evaluate()) {
							await ExecuteQueue(data, token);
							return;
						}
					}
				}
				if (ElseData != null) {
					await ExecuteQueue(ElseData, token);
				}
				return;
			}
			return;
		}

		private async Task ExecuteQueue(Queue<ParsedData> parsedData, CancellationToken token) {
			int counter = 0;
			while (parsedData.Count > 0) {
				if (token.IsCancellationRequested) {
					return;
				}
				ParsedData data = parsedData.Dequeue();
				data.Variables = Variables;
				await data.Execute(token);
				parsedData.Enqueue(data);
				counter++;
				if (counter == parsedData.Count) {
					return;
				}
			}
		}

		public void AddElse(string line, StringReader reader) {
			List<string> lines = BlockParser.ParseBlock(reader);

			Queue<ParsedData> data = CommandParser.Parse(string.Join(Environment.NewLine, lines), CommandParser.Window, Variables);
			ElseData = data;
		}

		public override ParsedData Parse(string line, StringReader reader, Dictionary<string, object> variables) {
			return this;
		}
	}
}
