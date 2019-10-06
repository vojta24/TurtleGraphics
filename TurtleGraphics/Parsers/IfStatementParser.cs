using System;
using System.Collections.Generic;
using System.IO;
using Flee.PublicTypes;

namespace TurtleGraphics.Parsers {
	public class IfStatementParser {
		public static ConditionalData ParseIfBlock(string line, StringReader reader, VariableStore variables, int indentaion, int lineIndex, out int readLines) {

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

			if (line.IndexOf('(') == -1) {
				throw new ParsingException("If statement invalid syntax!", line);
			}

			if (line.IndexOf(')') == -1) {
				throw new ParsingException("If statement invalid syntax!", line);
			}

			mod = mod.Trim('(', ')');
			int equalsIndex = mod.IndexOf("=");

			if (equalsIndex == mod.LastIndexOf("=") && equalsIndex != -1 && mod[equalsIndex - 1] != '!') {
				throw new ParsingException("If statement invalid syntax (== for comparison)!", line);
			}

			mod = mod.Replace("==", "=");
			mod = mod.Replace("!=", "<>");

			ExpressionContext context = FleeHelper.GetExpression(variables);

			try {
				IGenericExpression<bool> ifCondition = context.CompileGeneric<bool>(mod);
				int linesRead = 0;
				if (!line.EndsWith("{")) {
					linesRead += BlockParser.ReadToBlock(reader, line);
				}
				int totalRead = linesRead + lineIndex;
				List<string> lines = BlockParser.ParseBlock(reader);
				linesRead += lines.Count;

				List<ParsedData> isStatement = new List<ParsedData>();

				Queue<ParsedData> data = CommandParser.Parse(string.Join(Environment.NewLine, lines), MainWindow.Instance, indentaion, ref totalRead, variables);
				isStatement.AddRange(data);
				readLines = linesRead;
				return new ConditionalData(line, ifCondition, data, variables, indentaion);
			}
			catch (ParsingException) {
				throw;
			}
			catch (Exception e) {
				throw new ParsingException("Invalid boolean expression!", line, e);
			}
		}
	}
}
