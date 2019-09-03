using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class CommandParser {

		private static MainWindow win;

		internal static async Task<Queue<Func<Task>>> Parse(string commands, MainWindow window) {
			string[] split = commands.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

			win = window;

			Queue<Func<Task>> ret = new Queue<Func<Task>>();
			StringReader reader = new StringReader(commands);
			Dictionary<string, object> vars = new Dictionary<string, object>() {
				{ "Width", win.DrawWidth },
				{ "Height", win.DrawHeight }
			};
			while (reader.Peek() != -1) {
				Func<Task> func = await ParseLine(reader.ReadLine(), reader, vars);
				if (func != null) {
					ret.Enqueue(func);
				}
			}

			return ret;
		}


		private static async Task<Func<Task>> ParseLine(string line, StringReader reader, Dictionary<string, object> variables) {
			if (string.IsNullOrWhiteSpace(line)) {
				return null;
			}
			string[] split = line.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

			switch (split[0]) {
				case "move": {
					Point p = await ParsePoint(split[1], variables);
					return async () => { await win.Draw(p); };
				}
				case "for": {
					ForLoopData data = await ParseForLoop(split[1], reader, variables);
					return async () => {
						for (int i = data.From; i < data.To; i++) {
							Func<Task> t = data.Queue.Dequeue();
							await t();
							data.Queue.Enqueue(t);
						}
					};
				}

				case "rotate": {
					double angle = ParseAngle(split[1], variables);
					return async () => win.Rotate(angle);
				}

				case "fwd": {
					double distance = ParseDistance(split[1], variables);
					return async () => await win.Forward(distance);
				}

				default: {
					throw new InvalidOperationException();
				}
			}
		}

		private static double ParseDistance(string v, Dictionary<string, object> variables) {
			foreach (var item in variables) {
				v = v.Replace(item.Key, item.Value.ToString());
			}
			return ParseDoubleExpression(v, variables);
		}

		private static double ParseAngle(string v, Dictionary<string, object> variables) {
			foreach (var item in variables) {
				v = v.Replace(item.Key, item.Value.ToString());
			}
			return ParseDoubleExpression(v, variables);
		}

		private static async Task<ForLoopData> ParseForLoop(string v, StringReader reader, Dictionary<string, object> variables) {
			string[] split = v.Split();
			string variable = split[0];
			string[] range = split[2].Split(new[] { ".." }, StringSplitOptions.None);
			int from = int.Parse(range[0]);
			if (range[1].EndsWith("{")) {
				range[1] = range[1].Remove(range[1].Length - 1, 1);
			}
			variables.Add(variable, from);

			int to = int.Parse(range[1]);
			string next = reader.ReadLine();
			List<string> lines = new List<string>();
			Queue<Func<Task>> inner = new Queue<Func<Task>>();
			while (!next.StartsWith("}")) {
				Func<Task> t = await ParseLine(next, reader, variables);
				inner.Enqueue(t);
				lines.Add(next);
				next = reader.ReadLine();
			}
			for (int i = from + 1; i < to; i++) {
				variables[variable] = i;
				foreach (string _line in lines) {
					Func<Task> t = await ParseLine(_line, reader, variables);
					inner.Enqueue(t);
				}
			}
			variables.Remove(variable);
			return new ForLoopData() { From = from, To = to, Var = variable, Queue = inner };
		}

		private static async Task<Point> ParsePoint(string v, Dictionary<string, object> variables) {
			string[] split = v.Split(',');
			if (v.StartsWith("(") && v.EndsWith(")")) {
				split[0] = split[0].Replace("(", "");
				split[1] = split[1].Replace(")", "");
			}

			double X = ParseDoubleExpression(split[0], variables);

			double Y = ParseDoubleExpression(split[1], variables);

			return new Point(X, Y);
		}

		private static double ParseDoubleExpression(string v, Dictionary<string, object> variables) {
			ExpressionContext context = new ExpressionContext();

			foreach (KeyValuePair<string, object> item in variables) {
				context.Variables.Add(item.Key, item.Value);
			}

			IGenericExpression<double> eGeneric = context.CompileGeneric<double>(v);

			double result = eGeneric.Evaluate();

			return result;
		}
	}
}