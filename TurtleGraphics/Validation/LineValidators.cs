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

			//test 20);
			//teset ToDeg(Sin(PI))); 
			//Func RandX(),Rand(20,50));
			//for int i = 0; i < 20, i++)
			//if True)


			if (split.Length < 2)
				return false;
			

			if (split[1].EndsWith("{"))
				return false;

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
	}
}
