using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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

			if (LineValidators.IsConditional(line)) {
				if (line.Contains("if")) {
					ConditionalData data = ParseIfBlock(line, reader, variables.Copy());
					return data;
				}
			}
			throw new NotImplementedException($"Unexpected squence: {line}");
		}

		private static ConditionalData ParseIfBlock(string line, StringReader reader, Dictionary<string, object> variables) {

			//if (i > 50) {
			//if (i <= 50) {
			//if(i > 50) {
			//if (i > 50){

			string mod = line.Remove(0, 2).TrimStart();

			//(i > 50) {
			//(i <= 50) {
			//(i > 50) {
			//(i == 50){

			mod = mod.Replace("{", "").Trim();


			mod = mod.Trim('(', ')');

			//Dumb
			mod = mod.Replace("==", "=");

			ExpressionContext context = new ExpressionContext();
			context.Imports.AddType(typeof(Math));
			context.Imports.AddType(typeof(ContextExtensions));


			foreach (var item in variables) {
				context.Variables[item.Key] = item.Value;
			}

			IGenericExpression<bool> ifCondition = context.CompileGeneric<bool>(mod);

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
					return new ConditionalData(line, ifCondition, new Queue<ParsedData>()) {
						Variables = variables.Copy(),
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

			List<ParsedData> singleIteration = new List<ParsedData>();

			Queue<ParsedData> data = Parse(string.Join(Environment.NewLine, lines), win, variables);
			singleIteration.AddRange(data);

			return new ConditionalData(line, ifCondition, data) {
				Variables = variables.Copy(),
			};

		}

		private static IDynamicExpression ParseExpression(string line, Dictionary<string, object> variables) {
			ExpressionContext context = new ExpressionContext();

			context.Imports.AddType(typeof(Math));
			context.Imports.AddType(typeof(ContextExtensions));


			foreach (KeyValuePair<string, object> item in variables) {
				context.Variables.Add(item.Key, item.Value);
			}

			return context.CompileDynamic(line);
		}
	}
}