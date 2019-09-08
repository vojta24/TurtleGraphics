using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class RotateParseData : ParsedData {

		private readonly MainWindow _window;
		private readonly IGenericExpression<double> _expression;
		private readonly FunctionCallInfo _info;

		public RotateParseData(MainWindow w, IGenericExpression<double> expression, FunctionCallInfo info, Dictionary<string, object> variables) {
			_window = w;
			_expression = expression;
			_info = info;
			Variables = variables;

			if (info.Arguments.Length == 2) {
				bool.TryParse(info.Arguments[1], out _setRotation);
			}
		}

		public double Angle { get; set; } = double.NaN;
		private bool _setRotation;
		public bool SetRotation { get => _setRotation; set => _setRotation = value; }

		public override Task Execute(CancellationToken token) {
			if (token.IsCancellationRequested) {
				return Task.CompletedTask;
			}

			if (_expression == null) {
				_window.Rotate(0, SetRotation);
				return Task.CompletedTask;
			}

			UpdateVars(_expression);
			Angle = _expression.Evaluate();
			_window.Rotate(Angle, SetRotation);
			return Task.CompletedTask;
		}

		public override ParsedData Parse(string line, StringReader reader, Dictionary<string, object> variables) {
			return this;
		}
	}
}
