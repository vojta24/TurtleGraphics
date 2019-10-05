using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class ConditionalData : ParsedData {

		public IGenericExpression<bool> IfCondition { get; set; }
		public Queue<ParsedData> IfData { get; set; }
		public Queue<ParsedData> ElseData { get; set; } = null;
		public IList<(IGenericExpression<bool>, Queue<ParsedData>)> ElseIfs { get; set; }
		public bool IsModifiable { get; set; } = true;
		public int LineIndex { get; set; }

		public override bool IsBlock => true;

		public override ParsedAction Action => ParsedAction.NONE;

		public override string Line { get; set; }

		public ConditionalData(string line, IGenericExpression<bool> ifCondition, Queue<ParsedData> data, Dictionary<string, object> variables) : base(variables, line, line) {
			IfCondition = ifCondition;
			IfData = data;
			Line = line;
		}

		public void AddElse(StringReader reader, string line) {
			if (!line.EndsWith("{")) {
				BlockParser.ReadToBlock(reader, line);
			}
			List<string> lines = BlockParser.ParseBlock(reader);

			Queue<ParsedData> data = CommandParser.Parse(string.Join(Environment.NewLine, lines), CommandParser.Window, Variables);
			ElseData = data;
		}

		public override IList<TurtleData> CompileBlock(CancellationToken token) {
			List<TurtleData> ret = new List<TurtleData>(4096);
			UpdateVars(IfCondition);

			if (IfCondition.Evaluate()) {
				ret.AddRange(CompileQueue(IfData, token));
			}
			else {
				if (ElseData != null) {
					ret.AddRange(CompileQueue(ElseData, token));
				}
			}
			return ret;
		}

		private IEnumerable<TurtleData> CompileQueue(Queue<ParsedData> data, CancellationToken token) {
			List<TurtleData> interData = new List<TurtleData>();
			ParsedData current;
			int counter = 0;
			if (data.Count == 0) {
				return interData;
			}

			while (data.Count > 0) {
				token.ThrowIfCancellationRequested();
				current = data.Dequeue();
				counter++;
				if (current.IsBlock) {
					interData.AddRange(current.CompileBlock(token));
				}
				else {
					interData.Add(current.Compile(token));
				}
				data.Enqueue(current);
				if (counter == data.Count) {
					return interData;
				}
			}
			throw new Exception();
		}

		public override TurtleData Compile(CancellationToken token) {
			throw new NotImplementedException();
		}
	}
}
