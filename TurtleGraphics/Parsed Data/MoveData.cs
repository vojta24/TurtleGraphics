﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class MoveData : ParsedData {

		private readonly IGenericExpression<double> x;
		private readonly IGenericExpression<double> y;

		public MoveData(string[] args, VariableStore variables, string line, int lineIndex) : base(variables, line, lineIndex, args) {
			ExpressionContext expression = FleeHelper.GetExpression(variables, LineIndex);
			string exceptionMessage = "";
			try {
				exceptionMessage = "Invalid expression for X coordinate!";
				x = expression.CompileGeneric<double>(args[0]);
				exceptionMessage = "Invalid expression for Y coordinate!";
				y = expression.CompileGeneric<double>(args[1]);
			}
			catch (Exception e) {
				throw new ParsingException(exceptionMessage, line, e);
			}
		}

		public override bool IsBlock => false;

		public override ParsedAction Action => ParsedAction.MoveTo;

		public override string Line { get; set; }

		public override TurtleData Compile(CancellationToken token) {
			token.ThrowIfCancellationRequested();
			Variables.Update(x, LineIndex);
			Variables.Update(y, LineIndex);

			return new TurtleData {
				MoveTo = new Point(x.Evaluate(), y.Evaluate()),
				Jump = true,
				Action = Action,
			};
		}

		public override IList<TurtleData> CompileBlock(CancellationToken token, int indent) {
			throw new NotImplementedException();
		}
	}
}