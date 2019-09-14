using System;
using System.Collections.Generic;
using System.Globalization;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public static class FleeHelper {
		public static ExpressionContext GetExpression(Dictionary<string, object> variables) {
			ExpressionContext c = new ExpressionContext();
			c.Imports.AddType(typeof(Math));
			c.Imports.AddType(typeof(ContextExtensions));
			c.Options.ParseCulture = CultureInfo.GetCultureInfo("en-us");

			c.Variables.AddRange(variables);

			return c;
		}
	}
}
