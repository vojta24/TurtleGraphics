using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Flee.PublicTypes;

namespace TurtleGraphics {

	public class ForwardParseData : ParsedData<double> {

		private readonly MainWindow _window;

		public ForwardParseData(MainWindow w) {
			_window = w;
		}

		public double Distance { get; set; }

		public override ParsedData Clone() {
			return new ForwardParseData(_window) {
				Distance = Exp.Evaluate(),
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

	public class RotateParseData : ParsedData<double> {

		private readonly MainWindow _window;

		public RotateParseData(MainWindow w) {
			_window = w;
		}

		public double Angle { get; set; }

		public override ParsedData Clone() {
			return new RotateParseData(_window) {
				Angle = Exp.Evaluate(),
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

	public class MoveParseData : ParsedData<Point> {

		private readonly MainWindow _window;

		public MoveParseData(MainWindow w) {
			_window = w;
		}

		public Point MoveTo { get; set; }

		public override ParsedData Clone() {
			return new MoveParseData(_window) {
				MoveTo = this.MoveTo,
				Variables = new Dictionary<string, object>(this.Variables),
				Line = this.Line,
				Value = this.Value,
				Exp = this.Exp
			};
		}

		public override async Task Execute() {
			await _window.Draw(MoveTo);
		}
	}


	public class ParsedData<T> : ParsedData {

		public IGenericExpression<T> Exp { get; set; }

		public Func<T> Value { get; set; }

		public override ParsedData Clone() {
			return new ParsedData<T> { Value = this.Value, Variables = new Dictionary<string, object>(this.Variables), Exp = this.Exp, Line = this.Line };
		}

		public override Task Execute() {
			if (Exp == null) {
				return Task.Run(Value);
			}
			else {
				return Task.Run(Exp.Evaluate);
			}
		}
	}

	public abstract class ParsedData {
		public virtual IGenericExpression<object> Exp { get; set; }

		public string Line { get; set; }

		public Dictionary<string, object> Variables { get; set; }

		public Func<object> Value { get; set; }

		public virtual Task Execute() => Task.Run(Value);

		public abstract ParsedData Clone();
	}
}
