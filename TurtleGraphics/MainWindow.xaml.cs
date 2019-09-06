using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Igor.Models;
using static TurtleGraphics.Helpers;

namespace TurtleGraphics {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged {

		#region Notifications

		public event PropertyChangedEventHandler PropertyChanged;

		private void Notify(string prop) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}

		#endregion

		public MainWindow() {
			InitializeComponent();
			RunCommand = new AsyncCommand(RunCommandAction);
			Init();
			Loaded += MainWindow_Loaded;
			DataContext = this;
		}

		private Path _currentPath;
		private PathFigure _currentFigure;
		private string _lastColor;
		private bool _lastPenDown;

		private string _color;
		private double _brushSize;
		private Point _startPoint;
		private ICommand _runCommand;
		private string _commands;
		private double _angle;
		private double _x;
		private double _y;
		private int _delay;
		private bool _penDown;
		private int _iterationCount;

		public int IterationCount { get => _iterationCount; set { _iterationCount = value; Notify(nameof(IterationCount)); } }
		public bool PenDown { get => _penDown; set { _penDown = value; Notify(nameof(PenDown)); } }
		public int Delay { get => _delay; set { _delay = value; Notify(nameof(Delay)); } }
		public double Y { get => _y; set { _y = value; Notify(nameof(Y)); } }
		public double X { get => _x; set { _x = value; Notify(nameof(X)); } }
		public double Angle { get => _angle; set { _angle = value; Notify(nameof(Angle)); } }
		public string Commands { get => _commands; set { _commands = value; Notify(nameof(Commands)); } }
		public ICommand RunCommand { get => _runCommand; set { _runCommand = value; Notify(nameof(RunCommand)); } }
		public Point StartPoint { get => _startPoint; set { _startPoint = value; Notify(nameof(StartPoint)); } }
		public double BrushSize { get => _brushSize; set { _brushSize = value; Notify(nameof(BrushSize)); } }
		public string Color { get => _color; set { _color = value; Notify(nameof(Color)); } }

		public double DrawWidth { get; set; }
		public double DrawHeight { get; set; }

		private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
			DrawWidth = DrawAreaX.ActualWidth;
			DrawHeight = DrawAreaY.ActualHeight;
			Loaded -= MainWindow_Loaded;
		}

		public async Task Draw(Point to) {
			await Dispatcher.Invoke(async () => {
				LineSegment l = new LineSegment {
					IsStroked = true,
					Point = new Point(X, Y),
					IsSmoothJoin = true
				};

				if (_currentPath == null) {
					NewPath();
				}

				if (_lastPenDown != PenDown) {
					NewPath();
					_lastPenDown = PenDown;
				}

				if (_lastColor != Color) {
					NewPath();
					_lastColor = Color;
				}

				X = to.X;
				Y = to.Y;

				_currentFigure.Segments.Add(l);

				await Displace(to);
			});
		}

		public void NewPath() {
			_currentPath = new Path();

			if (!PenDown) {
				_currentPath.Stroke = Brushes.Transparent;
			}
			else {
				_currentPath.Stroke = (Brush)new BrushConverter().ConvertFromString(Color);
			}
			_currentPath.StrokeThickness = BrushSize;
			_currentPath.StrokeEndLineCap = PenLineCap.Round;
			_currentPath.StrokeStartLineCap = PenLineCap.Round;

			_currentPath.Data = new PathGeometry();
			(_currentPath.Data as PathGeometry).Figures = new PathFigureCollection();
			_currentFigure = new PathFigure();
			_currentFigure.StartPoint = new Point(X, Y);
			_currentFigure.Segments = new PathSegmentCollection();
			(_currentPath.Data as PathGeometry).Figures.Add(_currentFigure);
			Paths.Children.Add(_currentPath);
			Grid.SetColumn(_currentPath, 1);
		}

		public void Rotate(double angle, bool setRotation) {
			if (double.IsNaN(angle)) {
				Angle = 0;
				return;
			}
			if (setRotation) {
				Angle = ContextExtensions.AsRad(angle);
			}
			else {
				Angle += ContextExtensions.AsRad(angle);
			}
			if (Angle > 2 * Math.PI) {
				Angle -= 2 * Math.PI;
			}
		}

		public void SetPenDown(bool value) {
			PenDown = value;
		}

		public async Task Forward(double length) {
			double targetX = X + Math.Cos(Angle) * length;
			double targetY = Y + Math.Sin(Angle) * length;

			await Draw(new Point(targetX, targetY));
		}

		public async Task Displace(Point to) {
			int last = _currentFigure.Segments.Count - 1;
			LineSegment l = (LineSegment)_currentFigure.Segments[last];
			Point origin = l.Point;
			double increment = 1d / IterationCount;

			double currentInterpolation = 0;
			for (int i = 0; i <= IterationCount; i++) {
				l.Point = new Point(Lerp(origin.X, to.X, currentInterpolation), Lerp(origin.Y, to.Y, currentInterpolation));
				_currentFigure.Segments[last] = l;
				currentInterpolation += increment;
				await Task.Delay(Delay);
			}
			l.Freeze();
		}

		private async Task RunCommandAction() {
			//await Task.Delay(5000);

			Init();

			Queue<ParsedData> tasks = CommandParser.Parse(Commands, this);

			foreach (var item in tasks) {
				await item.Execute();
			}
		}

		public void Init() {
			List<UIElement> toRemove = new List<UIElement>();
			foreach (UIElement child in Paths.Children) {
				if (child.GetType() == typeof(Path)) {
					toRemove.Add(child);
				}
			}

			foreach (var item in toRemove) {
				Paths.Children.Remove(item);
			}
			_currentFigure = null;
			_currentPath = null;

			_lastColor = "";
			_lastPenDown = true;
			Color = "Blue";
			PenDown = true;
			X = 600;
			Y = 400;
			StartPoint = new Point(X, Y);
			Angle = 0;
			BrushSize = 5;
			IterationCount = 10;
			Delay = 5;
		}
	}
}
