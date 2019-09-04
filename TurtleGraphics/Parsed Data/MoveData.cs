using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flee.PublicTypes;

namespace TurtleGraphics {
	internal class MoveData : ParsedData {
		private readonly MainWindow _window;

		private readonly ExpressionContext expression = new ExpressionContext();

		private readonly IDynamicExpression x;
		private readonly IDynamicExpression y;

		public MoveData(MainWindow win, string input, Dictionary<string, object> variables) {
			_window = win;

			foreach (var item in variables) {
				expression.Variables[item.Key] = item.Value;
			}

			string[] split = input.Split(',');
			x = expression.CompileDynamic(split[0]);
			y = expression.CompileDynamic(split[1]);
		}

		public override IDynamicExpression Exp { get => base.Exp; set => base.Exp = value; }

		public override Task Execute() {
			double x_ = Convert.ToDouble(x.Evaluate());
			double y_ = Convert.ToDouble(y.Evaluate());

			_window.X = x_;
			_window.Y = y_;

			_window.NewPath();
			return Task.CompletedTask;
		}
	}
}