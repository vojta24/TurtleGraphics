using System;
using System.Collections;
using System.Collections.Generic;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class VariableStore : IEnumerable<KeyValuePair<string, object>> {

		private readonly Dictionary<string, object> data;
		private readonly List<(int, string)> layerKeys;
		private readonly Dictionary<string, int> accessibleSinceLine;
		public VariableStore Parent { get; set; }

		public object Get(string key, int myLine) {
			if (data.ContainsKey(key) && myLine > accessibleSinceLine[key]) {
				return data[key];
			}
			else if (Parent != null) {
				return Parent.Get(key, myLine);
			}
			throw new Exception("Not Found!");
		}

		public ICollection<string> Keys => data.Keys;

		public VariableStore() {
			Parent = null;
			data = new Dictionary<string, object>();
			accessibleSinceLine = new Dictionary<string, int>();
			layerKeys = new List<(int, string)>();
		}

		public VariableStore(VariableStore parent) {
			Parent = parent;
			data = new Dictionary<string, object>();
			layerKeys = new List<(int, string)>();
			accessibleSinceLine = new Dictionary<string, int>();
		}

		public void Add(string key, object value, int indentation, int accessibleSince) {
			if (data.ContainsKey(key)) {
				data[key] = value;
			}
			else {
				data.Add(key, value);
				layerKeys.Add((indentation, key));
				accessibleSinceLine.Add(key, accessibleSince);
			}
		}

		public bool ContainsVariable(string key) {
			if (data.ContainsKey(key)) {
				return true;
			}
			else if (Parent != null) {
				return Parent.ContainsVariable(key);
			}
			return false;
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
