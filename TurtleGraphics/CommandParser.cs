using System;
using System.Collections.Generic;
using System.IO;
using Flee.PublicTypes;
using TurtleGraphics.Parsers;
using TurtleGraphics.Validation;

namespace TurtleGraphics {
	public class CommandParser {

		public static MainWindow Window { get; set; }

		public static Queue<ParsedData> Parse(string commands, MainWindow window, Dictionary<string, object> additionalVars = null) {

			Window = window;
			conditionals.Clear();
			Dictionary<string, object> globalVars = new Dictionary<string, object>() {
				{ "Width", Window.DrawWidth },
				{ "Height", Window.DrawHeight }
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


		private static Stack<ConditionalData> conditionals = new Stack<ConditionalData>();

		private static ParsedData ParseLine(string line, StringReader reader, Dictionary<string, object> variables) {
			if (string.IsNullOrWhiteSpace(line) || line.Trim() == "}")
				return null;
			line = line.Trim();

			if (LineValidators.IsFunctionCall(line, out FunctionCallInfo info)) {
				if (conditionals.Count > 0) {
					conditionals.Peek().IsModifiable = false;
				}
				switch (info.FunctionName) {
					case "r": {
						double val;

						try {
							IDynamicExpression data = ParseExpression(info.Arguments[0], variables);
							val = Convert.ToDouble(data.Evaluate());
							bool hardAngle = false;

							if (info.Arguments.Length == 2) {
								hardAngle = bool.Parse(info.Arguments[1]);
							}
							return new RotateParseData(Window, val, hardAngle) {
								Variables = variables.Copy(),
								Exp = data,
								Line = line,
							};
						}
						catch {
							if (info.Arguments[0] == "origin") {
								val = double.NaN;
								return new RotateParseData(Window, val, bool.Parse(info.Arguments[1])) {
									Line = line,
									Variables = variables.Copy()
								};
							}
							throw;
						}
					}

					case "f": {
						IDynamicExpression data = ParseExpression(info.Arguments[0], variables);
						return new ForwardParseData(Window) {
							Variables = variables.Copy(),
							Exp = data,
							Line = line,
						};
					}

					case "SetBrushSize": {
						return new BrushSizeData(Window, ParseGenericExpression<double>(info.Arguments[0], variables)) {
							Variables = variables.Copy(),
							Line = line,
						};
					}

					case "u": {
						return new ActionData(() => Window.PenDown = false) { Variables = variables.Copy() };
					}

					case "d": {
						return new ActionData(() => Window.PenDown = true) { Variables = variables.Copy() };
					}

					case "c": {
						return new ColorData(Window, info.Arguments[0]) { Variables = variables.Copy() };
					}

					case "goto": {
						return new MoveData(Window, info.Arguments, variables.Copy());
					}

					default: {
						if (line == "}") {
							return null;
						}
						throw new NotImplementedException($"Unexpected squence function call: {line}");
					}
				}

			}

			if (LineValidators.IsForLoop(line)) {
				if (conditionals.Count > 0) {
					conditionals.Peek().IsModifiable = false;
				}
				ForLoopData data = ForLoopParser.ParseForLoop(line, reader, variables);
				data.Line = line;
				return data;
			}

			if (LineValidators.IsConditional(line)) {
				if (line.Contains("if")) {
					ConditionalData data = ParseIfBlock(line, reader, variables.Copy());
					conditionals.Push(data);
					return data;
				}
				//if (line.Contains("else if")) {

				//}
				if (line.Contains("else")) {
					ConditionalData latest = conditionals.Peek();
					latest.AddElse(line, reader);
					latest.IsModifiable = false;
					return null;
				}
			}
			throw new NotImplementedException($"Unexpected squence no category: {line}");
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
				if (next.Trim() == "}") {
					openBarckets--;
					if (openBarckets == 0) {
						lines.Add(next);
						break;
					}
				}
			}
			while (next != null);

			char c = (char)reader.Peek();

			List<ParsedData> singleIteration = new List<ParsedData>();

			Queue<ParsedData> data = Parse(string.Join(Environment.NewLine, lines), Window, variables);
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

		private static IGenericExpression<T> ParseGenericExpression<T>(string line, Dictionary<string, object> variables) {
			ExpressionContext context = new ExpressionContext();

			context.Imports.AddType(typeof(Math));
			context.Imports.AddType(typeof(ContextExtensions));


			foreach (KeyValuePair<string, object> item in variables) {
				context.Variables.Add(item.Key, item.Value);
			}

			return context.CompileGeneric<T>(line);
		}
	}
}