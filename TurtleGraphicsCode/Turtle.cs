using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using TurtleGraphics;

namespace TurtleGraphicsCode {
	/// <summary>
	/// Represents a Turtle. Provieds basic functions to work with turtle on a canvas.
	/// </summary>
	public class Turtle {

		private readonly List<TurtleData> _data = new List<TurtleData>();

		public TurtleGraphicsCodeData Data => new TurtleGraphicsCodeData() {
			Data = _data.ToArray(),
			ShowTurtle = ShowTurtle,
			AnimatePath = AnimatePath,
			PathAnimationSpeed = PathAnimationSpeed,
			TurtleSpeed = TurtleSpeed,
			BackgroundColor = BackgroundColor,
		};

		/// <summary>
		/// Run the application in full-screen mode
		/// </summary>
		public bool FullScreen { get; set; }

		/// <summary>
		/// Show the turtle while drawing
		/// </summary>
		public bool ShowTurtle { get; set; } = true;

		/// <summary>
		/// Get the screen width in pixels
		/// </summary>
		public double ScreenWidth => SystemParameters.PrimaryScreenWidth;

		/// <summary>
		/// Get the screen height in pixels
		/// </summary>
		public double ScreenHeight => SystemParameters.PrimaryScreenHeight;

		/// <summary>
		/// Current brush color
		/// </summary>
		public string BrushColor { get; private set; }

		/// <summary>
		/// Turtles current X position
		/// </summary>
		public double X { get; private set; }

		/// <summary>
		/// Turtles current Y position
		/// </summary>
		public double Y { get; private set; }

		/// <summary>
		/// Turltes current angle in radians
		/// </summary>
		public double Angle { get; private set; }

		/// <summary>
		/// Screen ceter point X coordinate
		/// </summary>
		public double MidX => ScreenWidth / 2;

		/// <summary>
		/// Screen center point Y coordinate
		/// </summary>
		public double MidY => ScreenHeight / 2;

		/// <summary>
		/// Animate the path the turtle is drawing
		/// </summary>
		public bool AnimatePath { get; set; }

		private int _pathAnimationSpeed = 1;

		/// <summary>
		/// The speed at which to draw the path
		/// </summary>
		public int PathAnimationSpeed {
			get => _pathAnimationSpeed;
			set {
				if (_pathAnimationSpeed == value) return;
				if (value < 1 || value > 20) throw new Exception("Only values between 1 and 20");
				_pathAnimationSpeed = value;
			}
		}

		private int _turtleSpeed = 1;

		/// <summary>
		/// The amount of frames calculated before rendering to the screen
		/// </summary>
		public int TurtleSpeed {
			get => _turtleSpeed;
			set {
				if (_turtleSpeed == value) return;
				if (value < 1 || value > 25) throw new Exception("Only values between 1 and 25");
				_turtleSpeed = value;
			}
		}


		public string BackgroundColor { get; set; }

		/// <summary>
		/// Create a new Turtle
		/// </summary>
		/// <param name="runFullscreen"></param>
		public Turtle(bool runFullscreen = false) {
			FullScreen = runFullscreen;
			X = ScreenWidth / 2;
			Y = ScreenHeight / 2;
		}

		/// <summary>
		/// Moves the turtle forward.
		/// </summary>
		/// <param name="distance">by given amount of pixels</param>
		public void Forward(double distance) {
			X += Math.Cos(Angle) * distance;
			Y += Math.Sin(Angle) * distance;
			_data.Add(new TurtleData() { Distance = distance, Action = ParsedAction.Forward });
		}

		/// <summary>
		/// Rotates the turtle by an angle.
		/// </summary>
		/// <param name="angle">the angle in degrees</param>
		/// <param name="setAngle">false = rotate by, true = rotate to</param>
		public void Rotate(double angle, bool setAngle = false) {
			if (setAngle) {
				Angle = angle * Math.PI / 180;
			}
			else {
				Angle += angle * Math.PI / 180;
			}
			_data.Add(new TurtleData() { Angle = angle, SetAngle = setAngle, Action = ParsedAction.Rotate });
		}

		/// <summary>
		/// Moves the turtle to given coordinates without drawing the path.
		/// </summary>
		/// <param name="x">the X coordinate</param>
		/// <param name="y">the Y coordinate</param>
		public void MoveTo(double x, double y) {
			X = x;
			Y = y;
			_data.Add(new TurtleData() { MoveTo = new Point(x, y), Action = ParsedAction.MoveTo });
		}

		/// <summary>
		/// Moves the turtle to given coordinates and draws the path.
		/// </summary>
		/// <param name="x">the X coordinate</param>
		/// <param name="y">the Y coordinate</param>
		public void DrawTo(double x, double y) {
			Vector directionalVec = new Vector(x, -y) - new Vector(X, -Y);
			Vector xAxisVec = new Vector(1, 0);
			double degreesBetween = Vector.AngleBetween(directionalVec, xAxisVec);

			Rotate(degreesBetween, true);
			Forward(directionalVec.Length);
		}

		/// <summary>
		/// Set the Brush color by providing all values.
		/// </summary>
		/// <param name="a">alpha component</param>
		/// <param name="r">red component</param>
		/// <param name="g">green component</param>
		/// <param name="b">blue component</param>
		public void SetColor(byte a, byte r, byte g, byte b) {
			_data.Add(new TurtleData() { Brush = new SolidColorBrush(new Color() { A = a, R = r, G = g, B = b }), Action = ParsedAction.Color });
		}

		/// <summary>
		/// Set the Brush color by providing all values.
		/// </summary>
		/// <param name="a">alpha component</param>
		/// <param name="r">red component</param>
		/// <param name="g">green component</param>
		/// <param name="b">blue component</param>
		public void SetColor(int a, int r, int g, int b) {
			SetColor((byte)a, (byte)r, (byte)g, (byte)b);
		}

		/// <summary>
		/// Set the Brush color by providing all values.
		/// </summary>
		/// <param name="a">alpha component</param>
		/// <param name="r">red component</param>
		/// <param name="g">green component</param>
		/// <param name="b">blue component</param>
		public void SetColor(double a, double r, double g, double b) {
			SetColor(Convert.ToByte(a), Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b));
		}

		/// <summary>
		/// Set the Brush color with full opacity.
		/// </summary>
		/// <param name="r">red component</param>
		/// <param name="g">green component</param>
		/// <param name="b">blue component</param>
		public void SetColor(byte r, byte g, byte b) {
			SetColor(byte.MaxValue, r, g, b);
		}

		/// <summary>
		/// Set the Brush color with full opacity.
		/// </summary>
		/// <param name="r">red component</param>
		/// <param name="g">green component</param>
		/// <param name="b">blue component</param>
		public void SetColor(int r, int g, int b) {
			SetColor(255, r, g, b);
		}

		/// <summary>
		/// Set the Brush color with full opacity.
		/// </summary>
		/// <param name="r">red component</param>
		/// <param name="g">green component</param>
		/// <param name="b">blue component</param>
		public void SetColor(double r, double g, double b) {
			SetColor(byte.MaxValue, Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b));
		}

		/// <summary>
		/// Set the Brush color with given string data.
		/// </summary>
		/// <param name="color">the predefined color name or hex value prefixed with '#'</param>
		public void SetColor(string color) {
			_data.Add(new TurtleData() { Brush = (SolidColorBrush)new BrushConverter().ConvertFromString(color), Action = ParsedAction.Color });
		}

		/// <summary>
		/// Set the brush size.
		/// </summary>
		/// <param name="size">the size in pixels</param>
		public void SetBrushSize(double size) {
			_data.Add(new TurtleData() { BrushThickness = size, Action = ParsedAction.Thickness });
		}

		/// <summary>
		/// Set the line's capping.
		/// </summary>
		/// <param name="lineCap">the finisher at the ends</param>
		public void SetLineCapping(PenLineCap lineCap) {
			_data.Add(new TurtleData() { LineCap = lineCap, Action = ParsedAction.Capping });
		}

		/// <summary>
		/// Raises the pen above the canvas.
		/// </summary>
		public void PenUp() {
			_data.Add(new TurtleData() { PenDown = false, Action = ParsedAction.PenState });
		}

		/// <summary>
		/// Lays the pen down onto the canvas.
		/// </summary>
		public void PenDown() {
			_data.Add(new TurtleData() { PenDown = true, Action = ParsedAction.PenState });
		}

		/// <summary>
		/// Store turtle's position and rotation.
		/// </summary>
		public void StoreTurtlePosition() {
			_data.Add(new TurtleData() { Action = ParsedAction.StorePos });
		}

		/// <summary>
		/// Restore turtle's position and rotation.
		/// </summary>
		/// <param name="pop">remove the entry from restore history</param>
		public void RestoreTurtlePosition(bool pop = false) {
			_data.Add(new TurtleData() { Action = ParsedAction.RestorePos, PopPosition = pop });
		}
	}
}
