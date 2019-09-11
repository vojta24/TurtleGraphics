using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public static class Helpers {
		public static double Lerp(double start, double end, double by) {
			return start * (1 - by) + end * by;
		}

		public static Dictionary<string, object> Join(Dictionary<string, object> a, Dictionary<string, object> b) {
			Dictionary<string, object> join = new Dictionary<string, object>(a);
			foreach (var item in b) {
				join[item.Key] = item.Value;
			}
			return join;
		}


		public static void AddRange(this VariableCollection a, Dictionary<string, object> b) {
			foreach (var item in b) {
				a[item.Key] = item.Value;
			}
		}

		public static T FindDescendant<T>(DependencyObject obj) where T : DependencyObject {
			if (obj == null) return default;
			int numberChildren = VisualTreeHelper.GetChildrenCount(obj);
			if (numberChildren == 0) return default;

			for (int i = 0; i < numberChildren; i++) {
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				if (child is T) {
					return (T)child;
				}
			}

			for (int i = 0; i < numberChildren; i++) {
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				var potentialMatch = FindDescendant<T>(child);
				if (potentialMatch != default(T)) {
					return potentialMatch;
				}
			}

			throw new Exception($"{nameof(T)} not found in {obj}!");
		}
	}
}
