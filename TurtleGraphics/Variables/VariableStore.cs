using System;
using System.Collections;
using System.Collections.Generic;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class VariableStore : IEnumerable<KeyValuePair<string, Variable>> {

		private readonly Dictionary<string, Variable> data;

		public VariableStore Parent { get; set; }

		public object Get(string key, int myLine) {
			return data[key].GetValue(myLine);
		}

		public ICollection<string> Keys => data.Keys;

		public VariableStore() {
			Parent = null;
			data = new Dictionary<string, Variable>();
		}

		public VariableStore(VariableStore parent) {
			Parent = parent;
			data = new Dictionary<string, Variable>();
		}

		public void Add(string key, object value, int accessibleSince, bool isUpdate) {
			if (isUpdate && data.ContainsKey(key)) {
				data[key].UpdateValue(accessibleSince, value);
			}
			else if (isUpdate && !data.ContainsKey(key)) {
				VariableStore parent = Parent;
				while (parent != null) {
					if (parent.data.ContainsKey(key)) {
						parent.data[key].UpdateValue(accessibleSince, value);
						return;
					}
					parent = parent.Parent;
				}
			}
			else {
				data.Add(key, new Variable(key, accessibleSince,value));
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

		public bool ContainsAccessibleVariable(string key, int lineIndex) {
			if (data.ContainsKey(key) && lineIndex > data[key].DefinitionLine) {
				return true;
			}
			else if (Parent != null) {
				return Parent.ContainsAccessibleVariable(key, lineIndex);
			}
			return false;
		}

		public IEnumerator<KeyValuePair<string, Variable>> GetEnumerator() {
			return data.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return data.GetEnumerator();
		}

		public void Update<T>(IGenericExpression<T> exp, int lineIndex) {
			exp.Context.Variables.AddRange(data,lineIndex);
			if (Parent != null) {
				Parent.Update(exp, lineIndex);
			}
		}

		public void Update(IDynamicExpression exp, int lineIndex) {
			exp.Context.Variables.AddRange(data, lineIndex);
			if (Parent != null) {
				Parent.Update(exp, lineIndex);
			}
		}

		internal void Update(ExpressionContext c, int lineIndex) {
			c.Variables.AddRange(data, lineIndex);
			if (Parent != null) {
				Parent.Update(c, lineIndex);
			}
		}
	}
}
