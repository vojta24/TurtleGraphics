using System;
using System.Collections.Generic;
using System.Threading;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class ForLoopData : ParsedData {
		public IGenericExpression<int> From { get; set; }
		public IGenericExpression<int> To { get; set; }
		public string LoopVariable { get; set; }
		public IGenericExpression<int> Change { get; set; }
		public OperatorType Operator { get; set; }
		public ConditionType Condition { get; set; }

		public List<string> Lines { get; set; }

		public override bool IsBlock => true;

		public override ParsedAction Action => ParsedAction.NONE;
		public override string Line { get; set; }

		public ForLoopData(VariableStore dictionary, string line, int lineIndex) : base(dictionary, line, lineIndex) { }

		public override TurtleData Compile(CancellationToken token) {
			throw new NotImplementedException();
		}

		public override IList<TurtleData> CompileBlock(CancellationToken token, int indent) {
			List<TurtleData> ret = new List<TurtleData>(4096);
			List<ParsedData> loopContents = CompileLoop(indent);

			ret.AddRange(CompileQueue(loopContents, token, indent));
			Variables.OnBlockLeave(indent);
			return ret;
		}

		private IEnumerable<TurtleData> CompileQueue(List<ParsedData> data, CancellationToken token, int indentation) {
			List<TurtleData> interData = new List<TurtleData>();
			ParsedData current;

			UpdateVars(From);
			UpdateVars(To);

			int FromInt = From.Evaluate();
			int ToInt = To.Evaluate();
			int ChangeInt = 1;
			if (Operator == OperatorType.MinusEquals || Operator == OperatorType.PlusEquals) {
				UpdateVars(Change);
				ChangeInt = Change.Evaluate();
			}

			void Exec(int i) {
				token.ThrowIfCancellationRequested();
				for (int counter = 0; counter < data.Count; counter++) {
					current = data[counter];

					current.Variables.Add(LoopVariable, i, indentation);

					if (current.IsBlock) {
						interData.AddRange(current.CompileBlock(token, indentation += 1));
					}
					else {
						TurtleData compiled = current.Compile(token);
						if (compiled.Action != ParsedAction.NONE) {
							interData.Add(compiled);
						}
					}
				}
			}

			switch (Condition) {
				case ConditionType.Greater: {
					if (Operator == OperatorType.PlusPlus) {
						for (int i = FromInt; i > ToInt; i++) {
							Exec(i);
						}
					}
					if (Operator == OperatorType.PlusEquals) {
						for (int i = FromInt; i > ToInt; i += ChangeInt) {
							Exec(i);
						}
					}
					if (Operator == OperatorType.MinMin) {
						for (int i = FromInt; i > ToInt; i--) {
							Exec(i);
						}
					}
					if (Operator == OperatorType.MinusEquals) {
						for (int i = FromInt; i > ToInt; i -= ChangeInt) {
							Exec(i);
						}
					}
					break;
				}
				case ConditionType.Less: {
					if (Operator == OperatorType.PlusPlus) {
						for (int i = FromInt; i < ToInt; i++) {
							Exec(i);
						}
					}
					if (Operator == OperatorType.PlusEquals) {
						for (int i = FromInt; i < ToInt; i += ChangeInt) {
							Exec(i);
						}
					}
					if (Operator == OperatorType.MinMin) {
						for (int i = FromInt; i < ToInt; i--) {
							Exec(i);
						}
					}
					if (Operator == OperatorType.MinusEquals) {
						for (int i = FromInt; i < ToInt; i -= ChangeInt) {
							Exec(i);
						}
					}
					break;
				}
				case ConditionType.GreaterOrEqual: {
					if (Operator == OperatorType.PlusPlus) {
						for (int i = FromInt; i >= ToInt; i++) {
							Exec(i);
						}
					}
					if (Operator == OperatorType.PlusEquals) {
						for (int i = FromInt; i >= ToInt; i += ChangeInt) {
							Exec(i);
						}
					}
					if (Operator == OperatorType.MinMin) {
						for (int i = FromInt; i >= ToInt; i--) {
							Exec(i);
						}
					}
					if (Operator == OperatorType.MinusEquals) {
						for (int i = FromInt; i >= ToInt; i -= ChangeInt) {
							Exec(i);
						}
					}
					break;
				}
				case ConditionType.LessOrEqual: {
					if (Operator == OperatorType.PlusPlus) {
						for (int i = FromInt; i <= ToInt; i++) {
							Exec(i);
						}
					}
					if (Operator == OperatorType.PlusEquals) {
						for (int i = FromInt; i <= ToInt; i += ChangeInt) {
							Exec(i);
						}
					}
					if (Operator == OperatorType.MinMin) {
						for (int i = FromInt; i <= ToInt; i--) {
							Exec(i);
						}
					}
					if (Operator == OperatorType.MinusEquals) {
						for (int i = FromInt; i <= ToInt; i -= ChangeInt) {
							Exec(i);
						}
					}
					break;
				}
			}
			return interData;
		}

		private List<ParsedData> CompileLoop(int indentation) {
			List<ParsedData> singleIteration = new List<ParsedData>();
			Variables.Add(LoopVariable, 0, indentation);
			Queue<ParsedData> data = CommandParser.Parse(
				string.Join(Environment.NewLine, Lines),
				CommandParser.Window,
				indentation,
				Variables);

			singleIteration.AddRange(data);

			return singleIteration;
		}
	}
}
