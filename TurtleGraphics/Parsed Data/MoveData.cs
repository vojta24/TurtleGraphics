using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class MoveData : ParsedData {

		private readonly ExpressionContext expression = new ExpressionContext();

		private readonly IGenericExpression<double> x;
		private readonly IGenericExpression<double> y;

		public MoveData(string[] args, Dictionary<string, object> variables) : base(args) {
			Variables = variables;

			expression.Imports.AddType(typeof(Math));
			expression.Imports.AddType(typeof(ContextExtensions));


			foreach (var item in variables) {
				expression.Variables[item.Key] = item.Value;
			}

			x = expression.CompileGeneric<double>(args[0]);
			y = expression.CompileGeneric<double>(args[1]);
		}

		public override bool IsBlock => false;

		public override ParsedAction Action => ParsedAction.MoveTo;

		public override TurtleData Compile(TurtleData previous, CancellationToken token) {
			token.ThrowIfCancellationRequested();
			UpdateVars(x);
			UpdateVars(y);

			return new TurtleData {
				Angle = previous.Angle,
				SetAngle = previous.SetAngle,
				Brush = previous.Brush,
				BrushThickness = previous.BrushThickness,
				PenDown = previous.PenDown,
				MoveTo = new Point(x.Evaluate(), y.Evaluate()),
				Jump = true,
				Action = Action,
			};
		}

		public override IList<TurtleData> CompileBlock(TurtleData previous, CancellationToken token) {
			throw new NotImplementedException();
		}
	}
}