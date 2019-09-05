using System;
using System.Collections.Generic;
using System.IO;
using Flee.PublicTypes;
using TurtleGraphics.Parsers;
using TurtleGraphics.Validation;

namespace TurtleGraphics {
	public class CommandParser {

		public static MainWindow win;

		public static Queue<ParsedData> Parse(string commands, MainWindow window, Dictionary<string, object> additionalVars = null) {

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
			if (string.IsNullOrWhiteSpace(line) || line.Trim() == "}")
				return null;
			line = line.Trim();

			if (LineValidators.IsFunctionCall(line, out FunctionCallInfo info)) {
				switch (info.FunctionName) {
					case "r": {
						double val;

						try {
							IDynamicExpression data = ParseExpression(info.Arguments[0], variables);
							val = Convert.ToDouble(data.Evaluate());
							return new RotateParseData(win) {
								Angle = val,
								Variables = variables.Copy(),
								Exp = data,
								Line = line,
							};
						}
						catch {
							if (info.Arguments[0] == "origin") {
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
						IDynamicExpression data = ParseExpression(info.Arguments[0], variables);
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
						return new ColorData(win, info.Arguments[0]) { Variables = variables.Copy() };
					}

					case "goto": {
						return new MoveData(win, info.Arguments, variables.Copy());
					}

					default: {
						if (line == "}") {
							return null;
						}
						throw new NotImplementedException($"Unexpected squence: {line}");
					}
				}

			}

			if (LineValidators.IsForLoop(line)) {
				ForLoopData data = ForLoopParser.ParseForLoop(line, reader, variables);
				data.Line = line;
				return data;
			}

			//string[] split = line.Trim().Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

			//switch (split[0]) {
			//	//case "for": {
			//	//	ForLoopData data = ForLoopParser.ParseForLoop(split[1].Trim(), reader, variables);
			//	//	data.Line = line;
			//	//	return data;
			//	//}

			//	case "r": {
			//		double val;

			//		try {
			//			IDynamicExpression data = ParseExpression(split[1], variables);
			//			val = Convert.ToDouble(data.Evaluate());
			//			return new RotateParseData(win) {
			//				Angle = val,
			//				Variables = variables.Copy(),
			//				Exp = data,
			//				Line = line,
			//			};
			//		}
			//		catch {
			//			if (split[1] == "origin") {
			//				val = double.NaN;
			//				return new RotateParseData(win) {
			//					Angle = val,
			//					Line = line,
			//					Variables = variables.Copy()
			//				};
			//			}
			//			throw;
			//		}
			//	}

			//	case "f": {
			//		IDynamicExpression data = ParseExpression(split[1], variables);
			//		return new ForwardParseData(win) {
			//			Variables = variables.Copy(),
			//			Exp = data,
			//			Line = line,
			//		};
			//	}

			//	case "u": {
			//		return new ActionData(() => win.PenDown = false) { Variables = variables.Copy() };
			//	}

			//	case "d": {
			//		return new ActionData(() => win.PenDown = true) { Variables = variables.Copy() };
			//	}

			//	case "c": {
			//		return new ColorData(win, split[1]);
			//	}

			//	case "goto": {
			//		return new MoveData(win, split[1].Split(','), variables.Copy());
			//	}

			//	default: {
			//		if (split[0] == "}") {
			//			return null;
			//		}
			//	}
			//}
			throw new NotImplementedException($"Unexpected squence: {line}");

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