using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class ForwardParseData : ParsedData {

		private readonly MainWindow _window;
		private readonly IGenericExpression<double> _expression;

		public ForwardParseData(MainWindow w, IGenericExpression<double> expression, Dictionary<string, object> variables) {
			_window = w;
			_expression = expression;
			Variables = variables;
		}

		public double Distance { get; set; }

		public override bool IsBlock => false;

		public override ParsedAction Action => ParsedAction.Forward;

		public override TurtleData Compile(TurtleData previous, CancellationToken token) {
			UpdateVars(_expression);
			Distance = _expression.Evaluate();
			return new TurtleData {
				Angle = previous.Angle,
				SetAngle = previous.SetAngle,
				Distance = Distance,
				Brush = previous.Brush,
				BrushThickness = previous.BrushThickness,
				MoveTo = new Point(previous.MoveTo.X + Math.Cos(previous.Angle) * Distance, previous.MoveTo.Y + Math.Sin(previous.Angle) * Distance),
				PenDown = previous.PenDown,
				Jump = false,
				Action = Action,
			};
		}

		public override IList<TurtleData> CompileBlock(TurtleData previous, CancellationToken token) {
			throw new NotImplementedException();
		}

		public override async Task Execute(CancellationToken token) {
			if (token.IsCancellationRequested) {
				return;
			}
			UpdateVars(_expression);
			Distance = _expression.Evaluate();
			await _window.Forward(Distance);
		}

		public override ParsedData Parse(string line, StringReader reader, Dictionary<string, object> variables) {
			return this;
		}
	}
}
