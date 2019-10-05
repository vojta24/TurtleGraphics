using System;
using System.Collections.Generic;
using System.Windows;
using TurtleGraphics.Parsers;

namespace TurtleGraphics.Validation {
	public static class LineValidators {
		public static readonly Dictionary<string, Type> typeDict = new Dictionary<string, Type> {
			{ "int", typeof(int) },
			{ "long", typeof(long) },
			{ "Point", typeof(Point) },
		};

		public static bool IsAssignment(string line, out AssignmentInfo assignment) {
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

		public static bool IsType(string line, out Type t) {
			t = null;
			if (typeDict.ContainsKey(line)) {
				t = typeDict[line];
				return true;
			}
			return false;
		}

		public static bool IsFunctionCall(string line, out FunctionCallInfo info) {

			//test (20);
			//teset(ToDeg(Sin(PI))); 
			//Func(RandX(),Rand(20,50));
			//for (int i = 0; i < 20, i++)
			//if (True)

			info = null;
			string[] split = line.Split(new[] { '(' }, 2, StringSplitOptions.RemoveEmptyEntries);

			//test  20);
			//teset ToDeg(Sin(PI))); 
			//Func RandX(),Rand(20,50));
			//for int i = 0; i < 20, i++)
			//if True)


			if (split.Length < 2)
				return false;

			split[0] = split[0].TrimEnd();

			if (FunctionNames.Fuctions.Contains(split[0]) && !split[1].EndsWith(";")) {
				throw new ParsingException("Missing semicolon after a function call!", line);
			}

			split[1] = split[1].TrimEnd(';', ' ');

			//test 20)
			//teset ToDeg(Sin(PI)))
			//Func RandX(),Rand(20,50))
			//for int i = 0; i < 20, i++)
			//if True)

			split[1] = split[1].Remove(split[1].Length - 1, 1);

			//test 20
			//teset ToDeg(Sin(PI))
			//Func RandX(),Rand(20,50)
			//for int i = 0; i < 20, i++
			//if True

			if (IsType(split[1].Split()[0], out _))
				return false;

			if (split[0].TrimEnd() == "for" || split[0].TrimEnd() == "if")
				return false;

			FunctionCallInfo i = new FunctionCallInfo();
			i.FunctionName = split[0].TrimEnd();

			i.Arguments = ArgParser.Parse(split[1].TrimEnd());

			info = i;
			return true;
		}

		public static bool IsForLoop(string line) {
			string[] split = line.Split('(', ')');
			bool startsWithFor = line.StartsWith("for");

			if (!startsWithFor) return false;

			bool isValidType = IsType(split[1].Split()[0], out _);

			return split.Length >= 3 && startsWithFor && isValidType;
		}

		internal static bool IsConditional(string line) {
			string[] split = line.Split('(', ')');
			bool isIf = split.Length == 3 && (split[0].Trim() == "if");
			bool isElseIf = split.Length == 3 && (split[0].Trim() == "else if");
			bool isElse = split.Length == 1 && split[0].TrimEnd(' ', '{') == "else";
			return isIf || isElseIf || isElse;
		}

		internal static bool IsVariableDeclaration(string line, Dictionary<string, object> variables, out (string, string, string) data) {
			data = (null, null, null);
			string[] values = line.Split('=');
			values[0].Trim();
			values[1].Trim();
			if (values.Length != 2) { return false; }
			string[] typeName = values[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (typeName.Length == 2) {
				typeName[0].TrimEnd();
				typeName[1].TrimStart();
				if (!SupportedTypes.Types.Contains(typeName[0])) { return false; }
				if (typeName[1] == "Width" || typeName[1] == "Height" || variables.ContainsKey(typeName[1])) { throw new ParsingException($"'{typeName[1]}' is already defined in an outer scope!", line); }
				if (!values[1].EndsWith(";")) { return false; }
				values[1] = values[1].TrimEnd(' ', ';');
				data = (typeName[0], typeName[1], values[1]);
				return true;
			}
			else {
				//Variable assignment
				if (IsVariableAssignment(new string[] { typeName[0].TrimEnd(), values[1] }, line, variables, out (string, string) assignData)) {
					data = (null, assignData.Item1, assignData.Item2);
					return true;
				}
				return false;
			}
		}

		private static bool IsVariableAssignment(string[] info, string line, Dictionary<string, object> variables, out (string, string) data) {
			if (!variables.ContainsKey(info[0])) { throw new ParsingException("Unable to assign value to an undefined variable!", line); }
			if (info[0] == "Width" || info[0] == "Height") { throw new ParsingException($"'{info[0]}' is a read-only variable!", line); }
			if (!info[1].EndsWith(";")) { throw new ParsingException($"Missing a semicolon at the end of variable assignment!", line); }
			info[1] = info[1].TrimEnd(' ', ';');
			data = (info[0].Trim(), info[1].TrimStart());
			return true;
		}
	}
}
