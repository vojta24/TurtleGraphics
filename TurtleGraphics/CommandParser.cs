using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class CommandParser {

		internal static MainWindow win;

		internal static Queue<ParsedData> Parse(string commands, MainWindow window, Dictionary<string, object> additionalVars = null) {
			string[] split = commands.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

			win = window;

			Queue<ParsedData> ret = new Queue<ParsedData>();
			StringReader reader = new StringReader(commands);
			Dictionary<string, object> vars = new Dictionary<string, object>() {
				{ "Width", win.DrawWidth },
				{ "Height", win.DrawHeight }
			};

			if(additionalVars != null) {
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


		private static ParsedData ParseLine(string line, StringReader reader, Dictionary<string, object> variables) {
			if (string.IsNullOrWhiteSpace(line)) {
				return null;
			}
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
					return new ColorData(win,split[1]);
				}

				default: {
					if(split[0] == "}") {
						return null;
					}
					throw new InvalidOperationException();
				}
			}
		}


		private static ForLoopData ParseForLoop(string v, StringReader reader, Dictionary<string, object> inherited) {
			string[] split = v.Split();
			string[] range = split[2].Split(new[] { ".." }, StringSplitOptions.None);
			if (range[1].EndsWith("{")) {
				range[1] = range[1].Remove(range[1].Length - 1, 1);
			}

			string variable = split[0];
			int from = int.Parse(range[0]);
			int to = int.Parse(range[1]);
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
					return new ForLoopData() { From = from, To = to, LoopVariable = variable, InheritedVariables = inherited, Exp = null, Line = v, Lines = lines };
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
					if(openBarckets == 0) {
						lines.Add(next);
						break;
					}
				}
			}
			while (next != null);

			return new ForLoopData() { From = from, To = to, LoopVariable = variable, InheritedVariables = inherited, Exp = null, Line = v, Lines = lines };
		}

		private static ParsedData ParseExpression(string v, Dictionary<string, object> variables) {
			ExpressionContext context = new ExpressionContext();

			foreach (KeyValuePair<string, object> item in variables) {
				context.Variables.Add(item.Key, item.Value);
			}

			IDynamicExpression result = context.CompileDynamic(v);
			return new ParsedData { Line = v, Exp = result };
		}
	}
}