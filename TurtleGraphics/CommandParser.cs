using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Flee.PublicTypes;
using TurtleGraphics.Parsers;

namespace TurtleGraphics {
	public class CommandParser {

		internal static MainWindow win;

		internal static readonly Dictionary<string, Type> typeDict = new Dictionary<string, Type> {
			{ "int", typeof(int) },
			{ "long", typeof(long) },
			{ "Point", typeof(Point) },
		};

		internal static Queue<ParsedData> Parse(string commands, MainWindow window, Dictionary<string, object> additionalVars = null) {

			win = window;
			Dictionary<string, object> globalVars = new Dictionary<string, object>() {
				{ "Width", win.DrawWidth },
				{ "Height", win.DrawHeight }
			};

			Queue<ParsedData> ret = new Queue<ParsedData>();
			using (StringReader reader = new StringReader(commands)) {

				if (additionalVars != null) {
					foreach (var item in additionalVars) {
						globalVars[item.Key] = item.Value;
					}
				}

				while (reader.Peek() != -1) {
					ParsedData data = ParseLine(reader.ReadLine(), reader, globalVars);
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
					ForLoopData data = ForLoopParser.ParseForLoop(split[1], reader, variables);
					data.Line = line;
					return data;
				}

				case "r": {
					double val;

					try {
						IDynamicExpression data = ParseExpression(split[1], variables);
						val = Convert.ToDouble(data.Evaluate());
						return new RotateParseData(win) {
							Angle = val,
							Variables = variables.Copy(),
							Exp = data,
							Line = line,
						};
					}
					catch {
						if (split[1] == "origin") {
							val = double.NaN;
							return new RotateParseData(win) {
								Angle = val,
								Line = line,
								Variables = variables.Copy()
							};
						}
						throw;
					}
				}

				case "f": {
					IDynamicExpression data = ParseExpression(split[1], variables);
					return new ForwardParseData(win) {
						Variables = variables.Copy(),
						Exp = data,
						Line = line,
					};
				}

				case "u": {
					return new ActionData(() => win.PenDown = false) { Variables = variables.Copy() };
				}

				case "d": {
					return new ActionData(() => win.PenDown = true) { Variables = variables.Copy() };
				}

				case "c": {
					return new ColorData(win, split[1]);
				}

				case "goto": {
					return new MoveData(win, split[1], variables.Copy());
				}

				default: {
					if (split[0] == "}") {
						return null;
					}
					throw new NotImplementedException($"Unexpected squence: {split[0]}");
				}
			}
		}

		private static bool IsAssignment(string line, out AssignmentInfo assignment) {
			assignment = null;

			string[] split = line.Split(new[] { ' ' }, 2);
			string[] type_name = split[0].Trim().Split();

			AssignmentInfo i = new AssignmentInfo();
			if (!IsType(type_name[0], out Type t)) {
				return false;
			}
			i.Type = t;
			i.VariableName = type_name[1];
			i.Value = split[1].TrimEnd(';');
			assignment = i;
			return true;
		}

		private static bool IsType(string line, out Type t) {
			t = null;
			if (typeDict.ContainsKey(line)) {
				t = typeDict[line];
				return true;
			}
			return false;
		}

		private static bool IsFunctionCall(string line) {
			string[] split = line.Split('(');
			string functionName = split[0].Trim();
			string[] parameters = split[1].TrimEnd(new[] { ';', ')' }).Split(',');
			return true;
		}

		private static IDynamicExpression ParseExpression(string line, Dictionary<string, object> variables) {
			ExpressionContext context = new ExpressionContext();

			foreach (KeyValuePair<string, object> item in variables) {
				context.Variables.Add(item.Key, item.Value);
			}

			return context.CompileDynamic(line);
		}
	}
}