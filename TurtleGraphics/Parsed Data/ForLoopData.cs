using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class ForLoopData : ParsedData {
		public int From { get; set; }
		public int To { get; set; }
		public string LoopVariable { get; set; }
		public int Change { get; set; }
		public OperatorType Operator { get; set; }
		public ConditionType Condition { get; set; }

		public List<string> Lines { get; set; }

		public override async Task Execute() {
			List<ParsedData> loopContents = CompileLoop();

			async Task Exec(int i) {
				foreach (ParsedData data in loopContents) {
					data.Variables[LoopVariable] = i;
					await data.Execute();
				}
			}

			switch (Condition) {
				case ConditionType.Greater: {
					if (Operator == OperatorType.PlusPlus) {
						for (int i = From; i > To; i++) {
							await Exec(i);
						}
					}
					if (Operator == OperatorType.PlusEquals) {
						for (int i = From; i > To; i += Change) {
							await Exec(i);
						}
					}
					if (Operator == OperatorType.MinMin) {
						for (int i = From; i > To; i--) {
							await Exec(i);
						}
					}
					if (Operator == OperatorType.MinusEquals) {
						for (int i = From; i > To; i -= Change) {
							await Exec(i);
						}
					}
					break;
				}
				case ConditionType.Less: {
					if (Operator == OperatorType.PlusPlus) {
						for (int i = From; i < To; i++) {
							await Exec(i);
						}
					}
					if (Operator == OperatorType.PlusEquals) {
						for (int i = From; i < To; i += Change) {
							await Exec(i);
						}
					}
					if (Operator == OperatorType.MinMin) {
						for (int i = From; i < To; i--) {
							await Exec(i);
						}
					}
					if (Operator == OperatorType.MinusEquals) {
						for (int i = From; i < To; i -= Change) {
							await Exec(i);
						}
					}
					break;
				}
				case ConditionType.GreaterOrEqual: {
					if (Operator == OperatorType.PlusPlus) {
						for (int i = From; i >= To; i++) {
							await Exec(i);
						}
					}
					if (Operator == OperatorType.PlusEquals) {
						for (int i = From; i >= To; i += Change) {
							await Exec(i);
						}
					}
					if (Operator == OperatorType.MinMin) {
						for (int i = From; i >= To; i--) {
							await Exec(i);
						}
					}
					if (Operator == OperatorType.MinusEquals) {
						for (int i = From; i >= To; i -= Change) {
							await Exec(i);
						}
					}
					break;
				}
				case ConditionType.LessOrEqual: {
					if (Operator == OperatorType.PlusPlus) {
						for (int i = From; i <= To; i++) {
							await Exec(i);
						}
					}
					if (Operator == OperatorType.PlusEquals) {
						for (int i = From; i <= To; i += Change) {
							await Exec(i);
						}
					}
					if (Operator == OperatorType.MinMin) {
						for (int i = From; i <= To; i--) {
							await Exec(i);
						}
					}
					if (Operator == OperatorType.MinusEquals) {
						for (int i = From; i <= To; i -= Change) {
							await Exec(i);
						}
					}
					break;
				}
			}


			//for (int i = From; i < To; i++) {
			//	foreach (ParsedData data in loopContents) {
			//		if (data.Exp != null) {
			//			data.Exp.Context.Variables[LoopVariable] = i;
			//		}
			//		await data.Execute();
			//	}
			//}
		}

		private List<ParsedData> CompileLoop() {
			List<ParsedData> singleIteration = new List<ParsedData>();

			Queue<ParsedData> data = CommandParser.Parse(string.Join(Environment.NewLine, Lines), CommandParser.Window, Helpers.Join(Variables, new Dictionary<string, object> { { LoopVariable, 0 } }));
			singleIteration.AddRange(data);

			return singleIteration;
		}
	}
}
