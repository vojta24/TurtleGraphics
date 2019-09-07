using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class MoveData : ParsedData {
		private readonly MainWindow _window;

		private readonly ExpressionContext expression = new ExpressionContext();

		private readonly IDynamicExpression x;
		private readonly IDynamicExpression y;

		public MoveData(MainWindow win, string[] args, Dictionary<string, object> variables) : base(args) {
			_window = win;
			Variables = variables;

			expression.Imports.AddType(typeof(Math));
			expression.Imports.AddType(typeof(ContextExtensions));


			foreach (var item in variables) {
				expression.Variables[item.Key] = item.Value;
			}

			x = expression.CompileDynamic(args[0]);
			y = expression.CompileDynamic(args[1]);
		}

		public override Task Execute(CancellationToken token) {
			if (token.IsCancellationRequested) {
				return Task.CompletedTask;
			}
			UpdateVars(x);
			UpdateVars(y);

			double x_ = Convert.ToDouble(x.Evaluate());
			double y_ = Convert.ToDouble(y.Evaluate());

			_window.X = x_;
			_window.Y = y_;

			_window.NewPath();
			return Task.CompletedTask;
		}
	}
}