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
		private static int lineIndex = 0;
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

		public static Queue<ParsedData> Parse(string commands, MainWindow window, VariableStore additionalVars = null) {
			Window = window;
			conditionals.Clear();
			VariableStore variables = null;
			if (additionalVars != null) {
				variables = additionalVars;
			}
			else {
				variables = new VariableStore {
					{ "Width", Window.DrawWidth, 0, false},
					{ "Height", Window.DrawHeight, 0, false }
				};
			}

			Queue<ParsedData> ret = new Queue<ParsedData>();
			using (StringReader reader = new StringReader(commands)) {

				//if (additionalVars != null) {
				//	foreach (var item in additionalVars) {
				//		variables.Add(item.Key, item.Value, lineIndex);
				//	}
				//}

				while (reader.Peek() != -1) {
					ParsedData data = ParseLine(reader.ReadLine(), reader, variables);
					lineIndex++;
					if (data != null) {
						ret.Enqueue(data);
					}
				}
				return ret;
			}
		}


		private static ParsedData ParseLine(string line, StringReader reader, VariableStore variables) {
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
						return new RotateParseData(ParseGenericExpression<double>(info.Arguments[0], LineIndexes[original], variables), info, variables, original, LineIndexes[original]);
					}

					case "Forward": {
						return new ForwardParseData(ParseGenericExpression<double>(info.Arguments[0], LineIndexes[original], variables), variables, original, LineIndexes[original]);
					}

					case "SetBrushSize": {
						return new BrushSizeData(ParseGenericExpression<double>(info.Arguments[0], LineIndexes[original], variables), variables, original, LineIndexes[original]);
					}

					case "PenUp": {
						return new PenPositionData(false, variables, original, LineIndexes[original]);
					}

					case "PenDown": {
						return new PenPositionData(true, variables, original, LineIndexes[original]);
					}

					case "SetColor": {
						return new ColorData(info.Arguments, variables, original, LineIndexes[original]);
					}

					case "MoveTo": {
						return new MoveData(info.Arguments, variables, original, LineIndexes[original]);
					}

					case "SetLineCapping": {
						return new BrushCappingData(info.Arguments, variables, original, LineIndexes[original]);
					}

					case "StoreTurtlePosition": {
						return new StoredPositionData(info.Arguments, variables, original, LineIndexes[original]);
					}

					case "RestoreTurtlePosition": {
						return new RestorePositionData(info.Arguments, variables, original, LineIndexes[original]);
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
				ForLoopData data = ForLoopParser.ParseForLoop(line, reader, new VariableStore(variables), LineIndexes[original]);
				return data;
			}

			if (LineValidators.IsConditional(line)) {
				if (line.Contains("if")) {
					ConditionalData data = IfStatementParser.ParseIfBlock(line, reader, new VariableStore(variables), LineIndexes[original]);
					conditionals.Push(data);
					return data;
				}
				if (line.Contains("else")) {
					ConditionalData latest = conditionals.Peek();
					latest.AddElse(reader, line, new VariableStore(variables), LineIndexes[original]);
					latest.IsModifiable = false;
					return null;
				}
			}

			if (LineValidators.IsVariableDeclaration(line, LineIndexes[original], variables, out (string, string, string) variableDef)) {
				if (conditionals.Count > 0) {
					conditionals.Peek().IsModifiable = false;
				}
				IDynamicExpression valueObj = ParseDynamicExpression(variableDef.Item3, LineIndexes[original], variables);
				object value = valueObj.Evaluate();
				variables.Add(variableDef.Item2, value, LineIndexes[original], variableDef.Item1 == null);
				return new VariableData(variableDef.Item2, valueObj, variableDef.Item1 != null, variables, line, LineIndexes[original]);
			}

			throw new ParsingException($"Unexpected squence!", line);
		}


		public static int GetLineIndex(string originalLine) {
			return LineIndexes[originalLine];
		}

		private static IGenericExpression<T> ParseGenericExpression<T>(string line, int lineIndex, VariableStore variables) {
			ExpressionContext context = FleeHelper.GetExpression(variables, lineIndex);
			try {
				return context.CompileGeneric<T>(line);
			}
			catch (Exception e) {
				throw new ParsingException($"Unable to parse expression of {typeof(T).FullName} from '{line}'", line, e);
			}
		}

		private static IDynamicExpression ParseDynamicExpression(string line, int lineIndex, VariableStore variables) {
			ExpressionContext context = FleeHelper.GetExpression(variables, lineIndex);
			try {
				return context.CompileDynamic(line);
			}
			catch (Exception e) {
				throw new ParsingException($"Unable to parse expression from '{line}'", line, e);
			}
		}
	}
}