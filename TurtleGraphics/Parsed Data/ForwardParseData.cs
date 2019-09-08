using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
