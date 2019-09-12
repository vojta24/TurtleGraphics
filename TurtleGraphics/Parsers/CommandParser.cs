using System;
using System.Collections.Generic;
using System.IO;
using Flee.PublicTypes;
using TurtleGraphics.Parsers;
using TurtleGraphics.Validation;

namespace TurtleGraphics {
	public class CommandParser {

		public static MainWindow Window { get; set; }
		private static readonly Stack<ConditionalData> conditionals = new Stack<ConditionalData>();

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


		private static ParsedData ParseLine(string line, StringReader reader, Dictionary<string, object> variables) {
			if (string.IsNullOrWhiteSpace(line) || line.Trim() == "}")
				return null;
			line = line.Trim();

			if (LineValidators.IsFunctionCall(line, out FunctionCallInfo info)) {
				if (conditionals.Count > 0) {
					conditionals.Peek().IsModifiable = false;
				}
				switch (info.FunctionName) {
					case "Rotate": {
						return new RotateParseData(ParseGenericExpression<double>(info.Arguments[0], variables), info, variables.Copy());
					}

					case "Forward": {
						return new ForwardParseData(ParseGenericExpression<double>(info.Arguments[0], variables), variables.Copy());
					}

					case "SetBrushSize": {
						return new BrushSizeData(ParseGenericExpression<double>(info.Arguments[0], variables), variables.Copy());
					}

					case "PenUp": {
						return new PenPositionData(false, variables.Copy());
					}

					case "PenDown": {
						return new PenPositionData(true, variables.Copy());
					}

					case "SetColor": {
						return new ColorData(info.Arguments[0], variables.Copy());
					}

					case "MoveTo": {
						return new MoveData(info.Arguments, variables.Copy());
					}

					default: {
						throw new ParsingException($"Unknown function: {line}");
					}
				}

			}

			if (LineValidators.IsForLoop(line)) {
				if (conditionals.Count > 0) {
					conditionals.Peek().IsModifiable = false;
				}
				ForLoopData data = ForLoopParser.ParseForLoop(line, reader, variables);
				return data;
			}

			if (LineValidators.IsConditional(line)) {
				if (line.Contains("if")) {
					ConditionalData data = IfStatementParser.ParseIfBlock(line, reader, variables.Copy());
					conditionals.Push(data);
					return data;
				}
				if (line.Contains("else")) {
					ConditionalData latest = conditionals.Peek();
					latest.AddElse(reader);
					latest.IsModifiable = false;
					return null;
				}
			}
			throw new ParsingException($"Unexpected squence at: {line}");
		}

		private static IGenericExpression<T> ParseGenericExpression<T>(string line, Dictionary<string, object> variables) {
			ExpressionContext context = new ExpressionContext();

			context.Imports.AddType(typeof(Math));
			context.Imports.AddType(typeof(ContextExtensions));


			foreach (KeyValuePair<string, object> item in variables) {
				context.Variables.Add(item.Key, item.Value);
			}
			try {
				return context.CompileGeneric<T>(line);
			}
			catch (Exception e) {
				throw new ParsingException($"Unable to parse expression of {typeof(T).FullName} from '{line}'", e);
			}
		}
	}
}