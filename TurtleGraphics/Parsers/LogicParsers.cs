using System;

namespace TurtleGraphics.Parsers {
	public static class LogicParsers {

		public static OperatorType ParseOperator(string str) {
			if (str.Contains(Operator.PLUSPLUS)) { return OperatorType.PlusPlus; }
			else if (str.Contains(Operator.PLUS_EQUALS)) { return OperatorType.PlusEquals; }
			else if (str.Contains(Operator.MINMIN)) { return OperatorType.MinMin; }
			else if (str.Contains(Operator.MIN_EQUALS)) { return OperatorType.MinusEquals; }
			throw new NotImplementedException($"{str} is not a valid Operator");
		}

		public static ConditionType ParseCondition(string str) {
			if (str.Contains(Condition.GREATER)) { return ConditionType.Greater; }
			else if (str.Contains(Condition.LESS)) { return ConditionType.Less; }
			else if (str.Contains(Condition.GREATER_OR_EQUAL)) { return ConditionType.GreaterOrEqual; }
			else if (str.Contains(Condition.LESS_OR_EQUAL)) { return ConditionType.LessOrEqual; }
			else if (str.Contains(Condition.EQUAL)) { return ConditionType.Equal; }
			throw new NotImplementedException($"{str} is not a valid Condition");
		}
	}
}
