using System.Collections.Generic;

namespace TurtleGraphics {
	public class Variable {

		public string Name { get; set; }

		public int DefinitionLine { get; set; }

		public object InitialValue { get; set; }


		public List<(int Index, object Value)> Changes { get; } = new List<(int, object)>();

		public Variable(string name, int definitionIndex, object initialValue) {
			Name = name;
			DefinitionLine = definitionIndex;
			InitialValue = initialValue;
		}

		public void AddValue(int lineIndex, object value) {
			Changes.Add((lineIndex, value));
		}

		public void UpdateValue(int lineIndex, object value) {
			(int, object) data = Changes.Find(p => p.Index == lineIndex);
			Changes.Remove(data);
			data.Item2 = value;
			Changes.Add(data);
		}

		public object GetValue(int lineIndex) {
			int nearestIndex = 0;
			object nearestValue = InitialValue;
			foreach ((int Index, object Val) in Changes) {
				if(Index > nearestIndex && lineIndex > Index) {
					nearestIndex = Index;
					nearestValue = Val;
				}
				if(lineIndex < Index) {
					return nearestValue;
				}
			}
			return nearestValue;
		}
	}
}
