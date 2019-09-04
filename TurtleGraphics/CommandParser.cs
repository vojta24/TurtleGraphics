using System;
using System.Collections.Generic;
using System.IO;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class CommandParser {

		internal static MainWindow win;

		internal static Queue<ParsedData> Parse(string commands, MainWindow window, Dictionary<string, object> additionalVars = null) {

			win = window;
			Dictionary<string, object> vars = new Dictionary<string, object>() {
				{ "Width", win.DrawWidth },
				{ "Height", win.DrawHeight }
			};

			Queue<ParsedData> ret = new Queue<ParsedData>();
			using (StringReader reader = new StringReader(commands)) {

				if (additionalVars != null) {
					foreach (var item in additionalVars) {
						vars[item.Key] = item.Value;
					}
				}

				while (reader.Peek() != -1) {
					ParsedData data = ParseLine(reader.ReadLine(), reader, vars);
					if (data != null) {
						ret.Enqueue(data);
					}
				}
				return ret;
			}
		}


		private static ParsedData ParseLine(string line, StringReader reader, Dictionary<string, object> variables) {
			if (string.IsNullOrWhiteSpace(line))
				return null;

			string[] split = line.Trim().Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

			switch (split[0]) {
				case "for": {
					ForLoopData data = ParseForLoop(split[1], reader, variables);
					data.Line = line;
					return data;
				}

				case "r": {
					ParsedData data = ParseExpression(split[1], variables);
					return new RotateParseData(win) {
						Angle = Convert.ToDouble(data.Exp.Evaluate()),
						Exp = data.Exp,
						Line = line,
					};
				}

				case "f": {
					ParsedData data = ParseExpression(split[1], variables);
					return new ForwardParseData(win) {
						Distance = Convert.ToDouble(data.Exp.Evaluate()),
						Exp = data.Exp,
						Line = line,
					};
				}

				case "u": {
					return new ActionData(() => win.PenDown = false);
				}

				case "d": {
					return new ActionData(() => win.PenDown = true);
				}

				case "c": {
					return new ColorData(win, split[1]);
				}

				default: {
					if (split[0] == "}") {
						return null;
					}
					throw new NotImplementedException($"Unexpected squence: {split[0]}");
				}
			}
		}


		private static ForLoopData ParseForLoop(string line, StringReader reader, Dictionary<string, object> inherited) {
			// (int i = 0; i < 50; i++) {
			// (int i=0;i<20;i++){
			// (long val=1; val <50; val+=2){

			line = line.Replace("(", "");

			// int i = 0; i < 50; i++) {
			// int i=0;i<20;i++){
			// long val=1; val <50; val+=2){

			line = line.Replace("int ", "").Replace("long ", "");

			// i = 0; i < 50; i++) {
			// i=0;i<20;i++){
			// val=1; val <50; val+=2){

			string variableName = line.Split('=')[0].Trim();
			line = line.Remove(0, line.IndexOf("=") + 1).Trim();

			// 0; i < 50; i++) {
			// 0 ;i<20;i++){
			// 1; val <50; val+=2){

			int startValue = int.Parse(line.Split(';')[0].Trim());
			line = line.Remove(0, line.IndexOf(";") + 1).Trim();

			// i < 50; i++) {
			// i<20 ;i++){
			// val >=50; val+=2){

			line = line.Remove(0, variableName.Length).Trim();

			// < 50; i++) {
			// <20 ;i++){
			// >=50; val+=2){

			string lhs = line.Split(';')[0].Trim();
			ConditionType condition = ParseCondition(lhs);


			if (lhs.Contains("=")) {
				line = line.Remove(0, 2).Trim();
			}
			else {
				line = line.Remove(0, 1).Trim();
			}

			string[] endValAndChange = line.Split(';');

			int endValue = int.Parse(endValAndChange[0].Trim());
			line = endValAndChange[1].Trim();

			// i++) {
			// i++){
			// val +=2){

			line = line.Remove(0, variableName.Length).Trim();

			// ++) {
			// ++){
			// +=2){

			OperatorType _operator = ParseOperator(line.Split(')')[0]);

			line = line.Remove(0, 2).Trim();

			// ) {
			// ){
			// 2){

			int change = 1;

			if (_operator == OperatorType.PlusEquals || _operator == OperatorType.MinusEquals) {
				string[] changeSplit = line.Split(')');
				change = int.Parse(changeSplit[0]);
				line = changeSplit[1].Trim();
			}

			// ) {
			// ){
			// ){

			line = line.Replace(")", "");
			line = line.Replace("{", "");

			if (!string.IsNullOrWhiteSpace(line)) {
				throw new Exception("What?");
			}

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
					return new ForLoopData() {
						From = startValue,
						To = endValue,
						LoopVariable = variableName,
						Change = change,
						Condition = condition,
						Operator = _operator,
						InheritedVariables = inherited,
						Exp = null,
						Line = line,
						Lines = lines
					};
				}
			}
			do {
				lines.Add(next);
				next = reader.ReadLine();
				if (next.Contains("{")) {
					openBarckets++;
				}
				if (next.Contains("}")) {
					openBarckets--;
					if (openBarckets == 0) {
						lines.Add(next);
						break;
					}
				}
			}
			while (next != null);

			return new ForLoopData() {
				From = startValue,
				To = endValue,
				LoopVariable = variableName,
				Change = change,
				Condition = condition,
				Operator = _operator,
				InheritedVariables = inherited,
				Exp = null,
				Line = line,
				Lines = lines
			};
		}

		private static OperatorType ParseOperator(string str) {
			if (str.Contains(Operator.PLUSPLUS)) { return OperatorType.PlusPlus; }
			else if (str.Contains(Operator.PLUS_EQUALS)) { return OperatorType.PlusEquals; }
			else if (str.Contains(Operator.MINMIN)) { return OperatorType.MinMin; }
			else if (str.Contains(Operator.MIN_EQUALS)) { return OperatorType.MinusEquals; }
			throw new NotImplementedException($"{str} is not a valid Operator");
		}

		private static ConditionType ParseCondition(string str) {
			if (str.Contains(Condition.GREATER)) { return ConditionType.Greater; }
			else if (str.Contains(Condition.LESS)) { return ConditionType.Less; }
			else if (str.Contains(Condition.GREATER_OR_EQUAL)) { return ConditionType.GreaterOrEqual; }
			else if (str.Contains(Condition.LESS_OR_EQUAL)) { return ConditionType.LessOrEqual; }
			else if (str.Contains(Condition.EQUAL)) { return ConditionType.Equal; }
			throw new NotImplementedException($"{str} is not a valid Condition");
		}

		private static ParsedData ParseExpression(string line, Dictionary<string, object> variables) {
			ExpressionContext context = new ExpressionContext();

			foreach (KeyValuePair<string, object> item in variables) {
				context.Variables.Add(item.Key, item.Value);
			}

			IDynamicExpression result = context.CompileDynamic(line);
			return new ParsedData { Line = line, Exp = result };
		}
	}
}