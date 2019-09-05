using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class ConditionalData : ParsedData {

		public IGenericExpression<bool> IfCondition { get; set; }
		public Func<Task> IfAction { get; set; }
		public IList<(IGenericExpression<bool>, Func<Task>)> ElseIfs { get; set; }
		public Func<Task> ElseAction { get; set; }

		public ConditionalData(string line,
			IGenericExpression<bool> ifCondition, Func<Task> if_,
			IList<(IGenericExpression<bool>, Func<Task>)> elseIfs = null,
			Func<Task> else_ = null) : base(line) {
			IfCondition = ifCondition;
			IfAction = if_;
			ElseIfs = elseIfs;
			ElseAction = else_;
		}

		public override async Task Execute() {
			if (IfCondition.Evaluate()) {
				await IfAction();
			}
			else {
				if (ElseIfs != null) {
					foreach ((IGenericExpression<bool> exp, Func<Task> act) in ElseIfs) {
						if (exp.Evaluate()) {
							await act();
							return;
						}
					}
				}
				 await ElseAction?.Invoke();
			}
			return;
		}
	}
}
