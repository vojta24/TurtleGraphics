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

		public override TurtleData Compile(TurtleData previous, CancellationToken token) {
			throw new NotImplementedException();
		}

		public override IList<TurtleData> CompileBlock(TurtleData previous, CancellationToken token) {
			List<TurtleData> ret = new List<TurtleData>(4096);
			List<ParsedData> loopContents = CompileLoop();

			ret.AddRange(CompileQueue(previous, loopContents, token));
			return ret;
		}

		private IEnumerable<TurtleData> CompileQueue(TurtleData previous, List<ParsedData> data, CancellationToken token) {
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
				for (int counter = 0; counter < data.Count; counter++) {
					current = data[counter];
					current.Variables[LoopVariable] = i;
					if (current.IsBlock) {
						interData.AddRange(current.CompileBlock(previous, token));
					}
					else {
						interData.Add(current.Compile(previous, token));
					}
					previous = interData[interData.Count - 1];
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

		private List<ParsedData> CompileLoop() {
			List<ParsedData> singleIteration = new List<ParsedData>();

			Queue<ParsedData> data = CommandParser.Parse(string.Join(Environment.NewLine, Lines), CommandParser.Window, Helpers.Join(Variables, new Dictionary<string, object> { { LoopVariable, 0 } }));
			singleIteration.AddRange(data);

			return singleIteration;
		}
	}
}
