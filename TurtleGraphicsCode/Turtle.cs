using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using TurtleGraphics;

namespace TurtleGraphicsCode {
	public class Turtle {

		private readonly List<TurtleData> _data = new List<TurtleData>();
		public TurtleGraphicsCodeData Data => new TurtleGraphicsCodeData() {
			Data = _data.ToArray(),
			ShowTurtle = ShowTurtle,
			AnimatePath = AnimatePath,
			PathAnimationSpeed = PathAnimationSpeed,
			TurtleSpeed = TurtleSpeed,
		};

		public bool FullScreen { get; private set; }

		public bool ShowTurtle { get; set; } = true;

		public bool AnimatePath { get; set; }

		private int _pathAnimationSpeed = 1;
		public int PathAnimationSpeed {
			get => _pathAnimationSpeed;
			set {
				if (_pathAnimationSpeed == value) return;
				if (value < 1 || value > 20) throw new Exception("Only values between 1 and 20");
				_pathAnimationSpeed = value;
			}
		}

		private int _turtleSpeed = 1;
		public int TurtleSpeed {
			get => _turtleSpeed;
			set {
				if (_turtleSpeed == value) return;
				if (value < 1 || value > 25) throw new Exception("Only values between 1 and 25");
				_turtleSpeed = value;
			}
		}

		public Turtle(bool runFullscreen = false) {
			FullScreen = runFullscreen;
		}

		public void Forward(double distance) {
			_data.Add(new TurtleData() { Distance = distance, Action = ParsedAction.Forward });
		}

		public void Rotate(double angle, bool setAngle = false) {
			_data.Add(new TurtleData() { Angle = angle, SetAngle = setAngle, Action = ParsedAction.Rotate });
		}

		public void MoveTo(double x, double y) {
			_data.Add(new TurtleData() { MoveTo = new Point(x, y), Action = ParsedAction.MoveTo });
		}

		public void SetBrushColor(byte a, byte r, byte g, byte b) {
			_data.Add(new TurtleData() { Brush = new SolidColorBrush(new Color() { A = a, R = r, G = g, B = b }), Action = ParsedAction.Color });
		}
		public void SetBrushColor(byte r, byte g, byte b) {
			_data.Add(new TurtleData() { Brush = new SolidColorBrush(new Color() { R = r, G = g, B = b }), Action = ParsedAction.Color });
		}
		public void SetBrushColor(string color) {
			_data.Add(new TurtleData() { Brush = (SolidColorBrush)new BrushConverter().ConvertFromString(color), Action = ParsedAction.Color });
		}

		public void SetBrushSize(double size) {
			_data.Add(new TurtleData() { BrushThickness = size, Action = ParsedAction.Thickness });
		}

		public void SetLineCapping(PenLineCap lineCap) {
			_data.Add(new TurtleData() { LineCap = lineCap, Action = ParsedAction.Capping });
		}

		public void PenUp() {
			_data.Add(new TurtleData() { PenDown = false, Action = ParsedAction.PenState });
		}

		public void PenDown() {
			_data.Add(new TurtleData() { PenDown = true, Action = ParsedAction.PenState });
		}

		public void StoreTurtlePosition() {
			_data.Add(new TurtleData() { Action = ParsedAction.StorePos });
		}

		public void RestoreTurtlePosition(bool pop = false) {
			_data.Add(new TurtleData() { Action = ParsedAction.RestorePos, PopPosition = pop });
		}
	}
}
