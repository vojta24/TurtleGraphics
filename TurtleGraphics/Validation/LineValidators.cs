using System;
using System.Collections.Generic;
using System.Windows;

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
			info = null;
			string[] split = line.Split(new[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
			if (split.Length != 3)
				return false;
			if (split[2].TrimStart() == "{")
				return false;
			if (IsType(split[1].Split()[0], out _))
				return false;
			FunctionCallInfo i = new FunctionCallInfo();
			i.FunctionName = split[0].TrimEnd();
			//TODO this can be a string!
			i.Arguments = split[1].Split(',');
			info = i;
			return true;
		}

		public static bool IsForLoop(string line) {
			string[] split = line.Split('(', ')');
			return split.Length == 3 && split[0].Trim() == "for";
		}
	}
}
