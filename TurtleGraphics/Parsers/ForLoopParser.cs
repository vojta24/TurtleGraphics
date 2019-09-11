using System;
using System.Collections.Generic;
using System.IO;
using Flee.PublicTypes;

namespace TurtleGraphics.Parsers {
	public class ForLoopParser {
		public static ForLoopData ParseForLoop(string line, StringReader reader, Dictionary<string, object> inherited) {

			ExpressionContext context = new ExpressionContext();
			context.Imports.AddType(typeof(Math));
			context.Imports.AddType(typeof(ContextExtensions));

			context.Variables.AddRange(inherited);

			// for(int i = 0; i < 50; i++) {
			// for (int i=0;i<20;i++){
			line = line.Remove(0, 3).Trim();


			// (int i = 0; i < 50; i++) {
			// (int i=0;i<20;i++){
			// (long val=1; val <50; val+=2){

			line = line.Replace("(", "");

			// int i = 0; i < 50; i++) {
			// int i=0;i<20;i++){
			// long val=1; val <50; val+=2){

			line = line.Replace("int ", "").Replace("long ", "");

			// i = 0; i < 50; i++) {
			// i=0;i<20;i++){
			// val=1; val <50; val+=2){

			string variableName = line.Split('=')[0].Trim();
			line = line.Remove(0, line.IndexOf("=") + 1).Trim();

			// 0; i < 50; i++) {
			// 0 ;i<20;i++){
			// 1; val <50; val+=2){

			//int startValue = int.Parse(line.Split(';')[0].Trim());
			IGenericExpression<int> startValueExp = context.CompileGeneric<int>(line.Split(';')[0].Trim());

			line = line.Remove(0, line.IndexOf(";") + 1).Trim();

			// i < 50; i++) {
			// i<20 ;i++){
			// val >=50; val+=2){

			line = line.Remove(0, variableName.Length).Trim();

			// < 50; i++) {
			// <20 ;i++){
			// >=50; val+=2){

			string lhs = line.Split(';')[0].Trim();
			ConditionType condition = LogicParsers.ParseCondition(lhs);


			if (lhs.Contains("=")) {
				line = line.Remove(0, 2).Trim();
			}
			else {
				line = line.Remove(0, 1).Trim();
			}

			string[] endValAndChange = line.Split(';');

			//int endValue = int.Parse(endValAndChange[0].Trim());
			IGenericExpression<int> endValueExp = context.CompileGeneric<int>(endValAndChange[0].Trim());

			line = endValAndChange[1].Trim();

			// i++) {
			// i++){
			// val +=2){

			line = line.Remove(0, variableName.Length).Trim();

			// ++) {
			// ++){
			// +=2){

			OperatorType _operator = LogicParsers.ParseOperator(line.Split(')')[0]);

			line = line.Remove(0, 2).Trim();

			// ) {
			// ){
			// 2){

			IGenericExpression<int> changeValueExp = null;


			if (_operator == OperatorType.PlusEquals || _operator == OperatorType.MinusEquals) {
				string[] changeSplit = line.Split(')');
				//change = int.Parse(changeSplit[0]);
				changeValueExp = context.CompileGeneric<int>(changeSplit[0]);
				line = changeSplit[1].Trim();
			}

			// ) {
			// ){
			// ){

			line = line.Replace(")", "");
			line = line.Replace("{", "");

			if (!string.IsNullOrWhiteSpace(line)) {
				throw new Exception("Follow the syntax pls?");
			}

			List<string> lines = BlockParser.ParseBlock(reader);

			return new ForLoopData() {
				From = startValueExp,
				To = endValueExp,
				LoopVariable = variableName,
				Change = changeValueExp,
				Condition = condition,
				Operator = _operator,
				Variables = inherited.Copy(),
				Lines = lines
			};
		}

	}
}
