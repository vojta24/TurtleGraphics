using System;
using System.Collections.Generic;
using System.IO;
using Flee.PublicTypes;

namespace TurtleGraphics.Parsers {
	public class IfStatementParser {
		public static ConditionalData ParseIfBlock(string line, StringReader reader, Dictionary<string, object> variables) {

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

			List<ParsedData> singleIteration = new List<ParsedData>();

			Queue<ParsedData> data = CommandParser.Parse(string.Join(Environment.NewLine, lines), MainWindow.Instance, variables);
			singleIteration.AddRange(data);

			return new ConditionalData(line, ifCondition, data) {
				Variables = variables.Copy(),
			};

		}

	}
}
