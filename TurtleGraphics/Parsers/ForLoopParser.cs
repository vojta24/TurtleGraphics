using System;
using System.Collections.Generic;
using System.IO;
using Flee.PublicTypes;

namespace TurtleGraphics.Parsers {
	public class ForLoopParser {
		public static ForLoopData ParseForLoop(string line, StringReader reader, Dictionary<string, object> inherited) {

			ExpressionContext context = FleeHelper.GetExpression(inherited);

			string errorMessage = "";

			try {
				// for(int i = 0; i < 50; i++) {
				// for (int i=0;i<20;i++){
				// for (int i=0;i<20;i++)
				string mod = line.Remove(0, 3).Trim();


				// (int i = 0; i < 50; i++) {
				// (int i=0;i<20;i++){
				// (int i=0;i<20;i++)
				// (long val=1; val <50; val+=2){
				errorMessage = "Invalid for loop syntax!";
				mod = mod.Remove(0, 1);

				// int i = 0; i < 50; i++) {
				// int i=0;i<20;i++){
				// int i=0;i<20;i++)
				// long val=1; val <50; val+=2){
				errorMessage = "Unsupported interation type!";
				mod = mod.Replace("int ", "").Replace("long ", "");

				// i = 0; i < 50; i++) {
				// i=0;i<20;i++){
				// i=0;i<20;i++)
				// val=1; val <50; val+=2){
				errorMessage = "Redefinition of variable!";
				string variableName = mod.Split('=')[0].Trim();

				if (inherited.ContainsKey(variableName)) {
					throw new ParsingException(errorMessage) { LineText = line };
				}

				mod = mod.Remove(0, mod.IndexOf("=") + 1).Trim();

				// 0; i < 50; i++) {
				// 0 ;i<20;i++){
				// 0 ;i<20;i++)
				// 1; val <50; val+=2){
				errorMessage = "Invalid expression for starting value!";
				IGenericExpression<int> startValueExp = context.CompileGeneric<int>(mod.Split(';')[0].Trim());

				errorMessage = "Invalid for loop syntax!";
				mod = mod.Remove(0, mod.IndexOf(";") + 1).Trim();

				// i < 50; i++) {
				// i<20 ;i++){
				// i<20 ;i++)
				// val >=50; val+=2){

				mod = mod.Remove(0, variableName.Length).Trim();

				// < 50; i++) {
				// <20 ;i++){
				// <20 ;i++)
				// >=50; val+=2){

				string lhs = mod.Split(';')[0].Trim();
				ConditionType condition = LogicParsers.ParseCondition(lhs);


				if (lhs.Contains("=")) {
					mod = mod.Remove(0, 2).Trim();
				}
				else {
					mod = mod.Remove(0, 1).Trim();
				}

				string[] endValAndChange = mod.Split(';');

				errorMessage = "Invalid expression for end value!";
				IGenericExpression<int> endValueExp = context.CompileGeneric<int>(endValAndChange[0].Trim());

				errorMessage = "Invalid for loop syntax!";
				mod = endValAndChange[1].Trim();

				// i++) {
				// i++){
				// i++)
				// val +=2){

				mod = mod.Remove(0, variableName.Length).Trim();

				// ++) {
				// ++){
				// ++)
				// +=2){

				OperatorType _operator = LogicParsers.ParseOperator(mod.Split(')')[0]);

				mod = mod.Remove(0, 2).Trim();

				// ) {
				// ){
				// )
				// 2){

				IGenericExpression<int> changeValueExp = null;


				if (_operator == OperatorType.PlusEquals || _operator == OperatorType.MinusEquals) {
					string[] changeSplit = mod.Split(')');
					errorMessage = "Invalid expression for change of value!";
					changeValueExp = context.CompileGeneric<int>(changeSplit[0]);
					mod = changeSplit[1].Trim();
				}

				// ) {
				// ){
				// )
				// ){

				mod = mod.Replace(")", "");

				// {
				//{
				if (!mod.Contains("{")) {
					BlockParser.ReadToBlock(reader, line);
				}
				List<string> lines = BlockParser.ParseBlock(reader);

				return new ForLoopData(inherited.Copy(), line) {
					From = startValueExp,
					To = endValueExp,
					LoopVariable = variableName,
					Change = changeValueExp,
					Condition = condition,
					Operator = _operator,
					Lines = lines,
				};
			}
			catch (ParsingException) {
				throw;
			}
			catch (Exception e) {
				throw new ParsingException(errorMessage, e) { LineText = line };
			}
		}
	}
}
