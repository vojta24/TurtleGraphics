using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Flee.PublicTypes;

namespace TurtleGraphics {

	public class ForwardParseData : ParsedData {

		private readonly MainWindow _window;

		public ForwardParseData(MainWindow w) {
			_window = w;
		}

		public double Distance { get; set; }

		public override async Task Execute() {
			Distance = Convert.ToDouble(Exp.Evaluate());
			await _window.Forward(Distance);
		}
	}

	public class RotateParseData : ParsedData {

		private readonly MainWindow _window;

		public RotateParseData(MainWindow w) {
			_window = w;
		}

		public double Angle { get; set; }

		public override Task Execute() {
			Angle = Convert.ToDouble(Exp.Evaluate());
			_window.Rotate(Angle);
			return Task.CompletedTask;
		}
	}

	public class ParsedData {
		public virtual IDynamicExpression Exp { get; set; }

		public string Line { get; set; }

		public virtual Task Execute() => throw new NotImplementedException();
	}
}
