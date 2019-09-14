using System;
using System.Collections.Generic;
using System.Globalization;
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

			if (line.IndexOf('{') == -1) {
				throw new ParsingException("If statement requires a block of code!");
			}

			mod = mod.Replace("{", "").Trim();

			if (line.IndexOf('(') == -1) {
				throw new ParsingException("If statement invalid syntax!");
			}

			if (line.IndexOf(')') == -1) {
				throw new ParsingException("If statement invalid syntax!");
			}

			mod = mod.Trim('(', ')');
			int equalsIndex = mod.IndexOf("=");

			if (equalsIndex == mod.LastIndexOf("=") && equalsIndex != -1) {
				throw new ParsingException("If statement invalid syntax (== for comparison)!");
			}

			mod = mod.Replace("==", "=");

			ExpressionContext context = FleeHelper.GetExpression(variables);

			try {
				IGenericExpression<bool> ifCondition = context.CompileGeneric<bool>(mod);
				List<string> lines = BlockParser.ParseBlock(reader);

				List<ParsedData> isStatement = new List<ParsedData>();

				Queue<ParsedData> data = CommandParser.Parse(string.Join(Environment.NewLine, lines), MainWindow.Instance, variables);
				isStatement.AddRange(data);

				return new ConditionalData(line, ifCondition, data) {
					Variables = variables.Copy(),
					Line = line,
				};
			}
			catch (ParsingException) {
				throw;
			}
			catch (Exception e) {
				throw new ParsingException("Invalid boolean expression!", e) { LineText = line };
			}
		}
	}
}
