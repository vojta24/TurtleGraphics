using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class ConditionalData : ParsedData {

		public IGenericExpression<bool> IfCondition { get; set; }
		public Queue<ParsedData> IfData { get; set; }
		public Queue<ParsedData> ElseData { get; set; } = null;
		public IList<(IGenericExpression<bool>, Func<Task>)> ElseIfs { get; set; }
		public bool IsModifiable { get; set; } = true;

		public ConditionalData(string line,
			IGenericExpression<bool> ifCondition, Queue<ParsedData> data,
			IList<(IGenericExpression<bool>, Func<Task>)> elseIfs = null) : base(line) {
			IfCondition = ifCondition;
			IfData = data;
			ElseIfs = elseIfs;
		}

		public override async Task Execute() {
			UpdateVars(IfCondition);
			if (IfCondition.Evaluate()) {
				await ExecuteQueue(IfData);
			}
			else {
				if (ElseIfs != null) {
					foreach ((IGenericExpression<bool> exp, Func<Task> act) in ElseIfs) {
						UpdateVars(exp);
						if (exp.Evaluate()) {
							await act();
							return;
						}
					}
				}
				if (ElseData != null) {
					await ExecuteQueue(ElseData);
				}
				return;
			}
			return;
		}

		private async Task ExecuteQueue(Queue<ParsedData> parsedData) {
			int counter = 0;
			while (parsedData.Count > 0) {
				ParsedData data = parsedData.Dequeue();
				data.Variables = Variables;
				await data.Execute();
				parsedData.Enqueue(data);
				counter++;
				if (counter == parsedData.Count) {
					return;
				}
			}
		}

		public void AddElse(string line, StringReader reader) {
			List<string> lines = new List<string>();
			string next = reader.ReadLine();
			int openBarckets = 1;

			if (next.Contains("{")) {
				openBarckets++;
			}
			if (next.Contains("}")) {
				openBarckets--;
				if (openBarckets == 0) {
					lines.Add(next);
					return;
				}
			}
			do {
				lines.Add(next);
				next = reader.ReadLine();
				if (next.Contains("{")) {
					openBarckets++;
				}
				if (next.Trim() == "}") {
					openBarckets--;
					if (openBarckets == 0) {
						lines.Add(next);
						break;
					}
				}
			}
			while (next != null);


			Queue<ParsedData> data = CommandParser.Parse(string.Join(Environment.NewLine, lines), CommandParser.Window, Variables);
			ElseData = data;
		}
	}
}
