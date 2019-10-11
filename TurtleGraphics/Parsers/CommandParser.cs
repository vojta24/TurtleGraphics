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
		public static Dictionary<string, int> LineIndexes = new Dictionary<string, int>();

		public static Queue<ParsedData> ParseCommands(string commands, MainWindow window) {
			LineIndexes.Clear();
			string[] split = commands.Replace("\r", "").Split('\n');
			for (int i = 0; i < split.Length; i++) {
				split[i] = split[i].Trim();

				if (LineIndexes.ContainsKey(split[i])) {
					int start = 1;
					while (true) {
						if (LineIndexes.ContainsKey($"{{{start}}}" + split[i])) {
							start++;
							continue;
						}
						LineIndexes.Add($"{{{start}}}" + split[i], i + 1);
						break;
					}
				}
				else {
					LineIndexes.Add(split[i], i + 1);
				}
			}

			commands = string.Join(Environment.NewLine, split);
			return Parse(commands, window);
		}

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
			string original = line;

			if (line.Length > 3) {
				if (line[0] == '{' && char.IsDigit(line[1]) && line[2] == '}') {
					line = line.Remove(0, 3);
				}
			}

			if (line.StartsWith("//")) {
				return null;
			}

			if (LineValidators.IsFunctionCall(line, out FunctionCallInfo info)) {
				if (conditionals.Count > 0) {
					conditionals.Peek().IsModifiable = false;
				}
				switch (info.FunctionName) {
					case "Rotate": {
						return new RotateParseData(ParseGenericExpression<double>(info.GetArg(0, line), line, variables), info, variables.Copy(), original);
					}

					case "Forward": {
						return new ForwardParseData(ParseGenericExpression<double>(info.GetArg(0, line), line, variables), variables.Copy(), original);
					}

					case "SetBrushSize": {
						return new BrushSizeData(ParseGenericExpression<double>(info.GetArg(0, line), line, variables), variables.Copy(), original);
					}

					case "PenUp": {
						return new PenPositionData(false, variables.Copy(), original);
					}

					case "PenDown": {
						return new PenPositionData(true, variables.Copy(), original);
					}

					case "SetColor": {
						return new ColorData(info.Arguments, variables.Copy(), original);
					}

					case "MoveTo": {
						return new MoveData(info.Arguments, variables.Copy(), original);
					}

					case "SetLineCapping": {
						return new BrushCappingData(info.Arguments, variables.Copy(), original);
					}

					case "StoreTurtlePosition": {
						return new StoredPositionData(info.Arguments, variables.Copy(), original);
					}

					case "RestoreTurtlePosition": {
						return new RestorePositionData(info.Arguments, variables.Copy(), original);
					}

					default: {
						throw new ParsingException($"Unknown function!", line);
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
					latest.AddElse(reader, line);
					latest.IsModifiable = false;
					return null;
				}
			}

			if (LineValidators.IsVariableDeclaration(line, variables, out (string, string, string) variableDef)) {
				IDynamicExpression valueObj = ParseDynamicExpression(variableDef.Item3, original, variables);
				object value = valueObj.Evaluate();
				variables[variableDef.Item2] = value;
				return new VariableData(variableDef.Item2, valueObj, variables, line);
			}

			throw new ParsingException($"Unexpected squence!", line);
		}

		private static IGenericExpression<T> ParseGenericExpression<T>(string line, string fullLine, Dictionary<string, object> variables) {
			ExpressionContext context = FleeHelper.GetExpression(variables);
			try {
				return context.CompileGeneric<T>(line);
			}
			catch (Exception e) {
				throw new ParsingException($"Unable to parse expression of {typeof(T).FullName} from '{line}'", line, e);
			}
		}

		private static IDynamicExpression ParseDynamicExpression(string line, string fullLine, Dictionary<string, object> variables) {
			ExpressionContext context = FleeHelper.GetExpression(variables);
			try {
				return context.CompileDynamic(line);
			}
			catch (Exception e) {
				throw new ParsingException($"Unable to parse expression from '{line}'", line, e);
			}
		}
	}
}