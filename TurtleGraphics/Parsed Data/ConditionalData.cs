using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class ConditionalData : ParsedData {

		public IGenericExpression<bool> IfCondition { get; set; }
		public Queue<ParsedData> IfData { get; set; }
		public VariableStore IfVariables { get; set; }
		public int IfLineIndex { get; set; }
		public Queue<ParsedData> ElseData { get; set; } = null;
		public VariableStore ElseVariables { get; set; }
		public int ElseLineIndex { get; set; }
		public IList<(IGenericExpression<bool>, Queue<ParsedData>)> ElseIfs { get; set; }
		public bool IsModifiable { get; set; } = true;

		public override bool IsBlock => true;

		public override ParsedAction Action => ParsedAction.NONE;

		public override string Line { get; set; }

		public int Indentaion { get; set; }

		public ConditionalData(string line, IGenericExpression<bool> ifCondition, Queue<ParsedData> data, VariableStore variables, int lineIndex) : base(variables, line, lineIndex, line) {
			IfCondition = ifCondition;
			IfData = data;
			IfLineIndex = lineIndex;
			Line = line;
			IfVariables = variables;
		}

		public void AddElse(StringReader reader, string line, VariableStore variables, int lineIndex) {
			if (!line.EndsWith("{")) {
				BlockParser.ReadToBlock(reader, line);
			}
			List<string> lines = BlockParser.ParseBlock(reader);

			Queue<ParsedData> data = CommandParser.Parse(string.Join(Environment.NewLine, lines), CommandParser.Window, Variables);
			ElseData = data;
			ElseVariables = variables;
			ElseLineIndex = lineIndex;
		}

		public override IList<TurtleData> CompileBlock(CancellationToken token, int indent) {
			List<TurtleData> ret = new List<TurtleData>(4096);
			Variables.Update(IfCondition, LineIndex);

			if (IfCondition.Evaluate()) {
				ret.AddRange(CompileQueue(IfData, IfVariables, token));
			}
			else {
				if (ElseData != null) {
					ret.AddRange(CompileQueue(ElseData, ElseVariables, token));
				}
			}
			return ret;
		}

		private IEnumerable<TurtleData> CompileQueue(Queue<ParsedData> data, VariableStore variables, CancellationToken token) {
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
					interData.AddRange(current.CompileBlock(token, Indentaion));
				}
				else {
					TurtleData compiled = current.Compile(token);
					if (compiled.Action != ParsedAction.NONE) {
						interData.Add(compiled);
					}
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
