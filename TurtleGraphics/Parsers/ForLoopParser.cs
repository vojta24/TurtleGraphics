using System;
using System.Collections.Generic;
using System.IO;

namespace TurtleGraphics.Parsers {
	public class ForLoopParser {
		public static ForLoopData ParseForLoop(string line, StringReader reader, Dictionary<string, object> inherited) {

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

			int startValue = int.Parse(line.Split(';')[0].Trim());
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

			int endValue = int.Parse(endValAndChange[0].Trim());
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

			int change = 1;

			if (_operator == OperatorType.PlusEquals || _operator == OperatorType.MinusEquals) {
				string[] changeSplit = line.Split(')');
				change = int.Parse(changeSplit[0]);
				line = changeSplit[1].Trim();
			}

			// ) {
			// ){
			// ){

			line = line.Replace(")", "");
			line = line.Replace("{", "");

			if (!string.IsNullOrWhiteSpace(line)) {
				throw new Exception("What?");
			}

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
					return new ForLoopData() {
						From = startValue,
						To = endValue,
						LoopVariable = variableName,
						Change = change,
						Condition = condition,
						Operator = _operator,
						Variables = inherited.Copy(),
						Exp = null,
						Line = line,
						Lines = lines
					};
				}
			}
			do {
				lines.Add(next);
				next = reader.ReadLine();
				if (next.Contains("{")) {
					openBarckets++;
				}
				if (next.Contains("}")) {
					openBarckets--;
					if (openBarckets == 0) {
						lines.Add(next);
						break;
					}
				}
			}
			while (next != null);

			return new ForLoopData() {
				From = startValue,
				To = endValue,
				LoopVariable = variableName,
				Change = change,
				Condition = condition,
				Operator = _operator,
				Variables = inherited.Copy(),
				Exp = null,
				Line = line,
				Lines = lines
			};
		}

	}
}
