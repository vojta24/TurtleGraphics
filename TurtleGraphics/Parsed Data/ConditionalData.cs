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

		public override bool IsBlock => true;

		public override ParsedAction Action => ParsedAction.NONE;

		public ConditionalData(string line,
			IGenericExpression<bool> ifCondition, Queue<ParsedData> data,
			IList<(IGenericExpression<bool>, Queue<ParsedData>)> elseIfs = null) : base(line) {
			IfCondition = ifCondition;
			IfData = data;
			ElseIfs = elseIfs;
		}

		public void AddElse(StringReader reader) {
			List<string> lines = BlockParser.ParseBlock(reader);

			Queue<ParsedData> data = CommandParser.Parse(string.Join(Environment.NewLine, lines), CommandParser.Window, Variables);
			ElseData = data;
		}

		public override IList<TurtleData> CompileBlock(TurtleData previous, CancellationToken token) {
			List<TurtleData> ret = new List<TurtleData>(4096);
			UpdateVars(IfCondition);

			if (IfCondition.Evaluate()) {
				ret.AddRange(CompileQueue(previous, IfData, token));
			}
			else {
				if (ElseData != null) {
					ret.AddRange(CompileQueue(previous, ElseData, token));
				}
			}
			return ret;
		}

		private IEnumerable<TurtleData> CompileQueue(TurtleData previous, Queue<ParsedData> data, CancellationToken token) {
			List<TurtleData> interData = new List<TurtleData>();
			ParsedData current;
			int counter = 0;
			while (data.Count > 0) {
				current = data.Dequeue();
				counter++;
				if (current.IsBlock) {
					interData.AddRange(current.CompileBlock(previous, token));
				}
				else {
					interData.Add(current.Compile(previous, token));
				}
				previous = interData[interData.Count - 1];
				data.Enqueue(current);
				if (counter == data.Count) {
					return interData;
				}
			}
			throw new Exception();
		}

		public override TurtleData Compile(TurtleData previous, CancellationToken token) {
			throw new NotImplementedException();
		}
	}
}
