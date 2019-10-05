using System;
using System.Collections;
using System.Collections.Generic;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class VariableStore : IEnumerable<KeyValuePair<string,object>> {

		private readonly Dictionary<string, object> data = new Dictionary<string, object>();
		private readonly List<(int, string)> layerKeys = new List<(int, string)>();

		public object this[string index] {
			get => data[index];
		}

		public ICollection<string> Keys => data.Keys;

		public void Add(string key, object value, int indentation) {
			if (data.ContainsKey(key)) {
				data[key] = value;
			}
			else {
				data.Add(key, value);
				layerKeys.Add((indentation, key));
			}
		}

		public bool ContainsVariable(string key) {
			return data.ContainsKey(key);
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
			return data.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return data.GetEnumerator();
		}

		public void OnBlockLeave(int layer) {
			foreach ((int _layer, string key) in layerKeys) {
				if (layer == _layer) {
					data.Remove(key);
				}
			}
			layerKeys.RemoveAll(p => p.Item1 == layer);
		}

		public void Update<T>(IGenericExpression<T> exp) {
			exp.Context.Variables.AddRange(data);
		}

		public void Update(IDynamicExpression exp) {
			exp.Context.Variables.AddRange(data);
		}

		internal void Update(ExpressionContext c) {
			c.Variables.AddRange(data);
		}

	}
}
