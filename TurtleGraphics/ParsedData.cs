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

		public override ParsedData Clone() {
			return new ForwardParseData(_window) {
				Distance = Convert.ToDouble(Exp.Evaluate()),
				Variables = new Dictionary<string, object>(this.Variables),
				Line = this.Line,
				Value = this.Value,
				Exp = this.Exp
			};
		}

		public override async Task Execute() {
			await _window.Forward(Distance);
		}
	}

	public class PointParsedData : ParsedData {

		Point Point { get; set; }

		public override ParsedData Clone() {
			throw new NotImplementedException();
		}
	}

	public class RotateParseData : ParsedData {

		private readonly MainWindow _window;

		public RotateParseData(MainWindow w) {
			_window = w;
		}

		public double Angle { get; set; }

		public override ParsedData Clone() {
			return new RotateParseData(_window) {
				Angle = Convert.ToDouble(Exp.Evaluate()),
				Variables = new Dictionary<string, object>(this.Variables),
				Line = this.Line,
				Value = this.Value,
				Exp = this.Exp
			};
		}

		public override Task Execute() {
			_window.Rotate(Angle);
			return Task.CompletedTask;
		}
	}

	//public class MoveParseData : ParsedData {

	//	private readonly MainWindow _window;

	//	public MoveParseData(MainWindow w) {
	//		_window = w;
	//	}

	//	public Point MoveTo { get; set; }

	//	public override ParsedData Clone() {
	//		return new MoveParseData(_window) {
	//			MoveTo = this.MoveTo,
	//			Variables = new Dictionary<string, object>(this.Variables),
	//			Line = this.Line,
	//			Value = this.Value,
	//			Exp = this.Exp
	//		};
	//	}

	//	public override async Task Execute() {
	//		await _window.Draw(MoveTo);
	//	}
	//}

	public class ParsedData {
		public virtual IDynamicExpression Exp { get; set; }

		public string Line { get; set; }

		public Dictionary<string, object> Variables { get; set; }

		public Func<object> Value { get; set; }

		public virtual Task Execute() => Task.Run(Value);

		public virtual ParsedData Clone() => throw new NotImplementedException();
	}
}
