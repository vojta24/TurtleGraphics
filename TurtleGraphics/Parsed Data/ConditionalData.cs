using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class ConditionalData : ParsedData {

		public IGenericExpression<bool> IfCondition { get; set; }
		public Queue<ParsedData> IfData { get; set; }
		public IList<(IGenericExpression<bool>, Func<Task>)> ElseIfs { get; set; }
		public Func<Task> ElseAction { get; set; }

		public ConditionalData(string line,
			IGenericExpression<bool> ifCondition, Queue<ParsedData> data,
			IList<(IGenericExpression<bool>, Func<Task>)> elseIfs = null,
			Func<Task> else_ = null) : base(line) {
			IfCondition = ifCondition;
			IfData = data;
			ElseIfs = elseIfs;
			ElseAction = else_;
		}

		public override async Task Execute() {
			UpdateVars(IfCondition);
			if (IfCondition.Evaluate()) {
				int counter = 0;
				while (IfData.Count > 0) {
					ParsedData data = IfData.Dequeue();
					data.Variables = Variables;
					await data.Execute();
					IfData.Enqueue(data);
					counter++;
					if (counter == IfData.Count) {
						return;
					}
				}
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
				if (ElseAction != null) {
					await ElseAction();
				}
				return;
			}
			return;
		}
	}
}
