using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Igor.Models;
using static TurtleGraphics.Helpers;
using System.Linq.Expressions;
using Flee.PublicTypes;
using System.Windows.Shapes;
using System.Windows.Controls;

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
			Top = 0;
			Left = 0;
			X = 600;
			Y = 400;
			StartPoint = new Point(X, Y);
			Loaded += (s, e) => { MainWindow_Loaded(s, e); };
			DataContext = this;
		}


		private Path currentPath;
		private PathFigure currentFigure;
		public double DrawWidth;
		public double DrawHeight;

		private string last_color = "";
		private bool lastPenDown = true;

		private string _color = "Blue";
		private double _brushSize = 5;
		private Point _startPoint;
		private ICommand _runCommand;
		private string _commands;
		private double _angle;
		private double _x;
		private double _y;
		private int _delay = 5;
		private bool _penDown = true;
		private int _iterationCount = 10;

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

		private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
			DrawWidth = DrawAreaX.ActualWidth;
			DrawHeight = DrawAreaY.ActualHeight;
		}

		public async Task Draw(Point to) {
			await Dispatcher.Invoke(async () => {
				LineSegment l = new LineSegment {
					IsStroked = true,
					Point = new Point(X, Y)
				};


				l.IsSmoothJoin = true;
				if (currentPath == null) {
					NewPath();
				}

				if (lastPenDown != PenDown) {
					NewPath();
					lastPenDown = PenDown;
				}

				if (last_color != Color) {
					NewPath();
					last_color = Color;
				}

				X = to.X;
				Y = to.Y;

				currentFigure.Segments.Add(l);

				await Displace(to);
			});
		}

		internal void NewPath() {
			currentPath = new Path();
			if (!PenDown) {
				currentPath.Stroke = Brushes.Transparent;
			}
			else {
				currentPath.Stroke = (Brush)new BrushConverter().ConvertFromString(Color);
			}
			currentPath.StrokeThickness = BrushSize;
			currentPath.StrokeEndLineCap = PenLineCap.Round;
			currentPath.StrokeStartLineCap = PenLineCap.Round;

			currentPath.Data = new PathGeometry();
			(currentPath.Data as PathGeometry).Figures = new PathFigureCollection();
			currentFigure = new PathFigure();
			currentFigure.StartPoint = new Point(X, Y);
			currentFigure.Segments = new PathSegmentCollection();
			(currentPath.Data as PathGeometry).Figures.Add(currentFigure);
			Paths.Children.Add(currentPath);
			Grid.SetColumn(currentPath, 1);
		}

		internal void Rotate(double angle) {
			Angle += Math.PI * angle / 180.0;
			if (Angle == 2 * Math.PI) {
				Angle = 0;
			}
		}

		internal void SetPenDown(bool value) {
			PenDown = value;
		}


		public async Task Forward(double length) {
			double targetX = X + Math.Cos(Angle) * length;
			double targetY = Y + Math.Sin(Angle) * length;

			await Draw(new Point(targetX, targetY));
		}

		public async Task Displace(Point to) {
			int last = currentFigure.Segments.Count - 1;
			LineSegment l = (LineSegment)currentFigure.Segments[last];
			Point origin = l.Point;
			double inc = 1d / IterationCount;
			double curr = 0;
			for (int i = 0; i <= IterationCount; i++) {
				l.Point = new Point(Lerp(origin.X, to.X, curr), Lerp(origin.Y, to.Y, curr));
				currentFigure.Segments[last] = l;
				await Task.Delay(Delay);
				curr += inc;
			}
			l.Freeze();
		}

		private async Task RunCommandAction() {
			List<UIElement> toRemove = new List<UIElement>();
			foreach (UIElement child in Paths.Children) {
				if (child.GetType() == typeof(Path)) {
					toRemove.Add(child);
				}
			}

			foreach (var item in toRemove) {
				Paths.Children.Remove(item);
			}
			currentFigure = null;
			currentPath = null;

			last_color = "";
			Color = "Blue";
			PenDown = true;
			X = 600;
			Y = 400;
			StartPoint = new Point(X, Y);
			Angle = 0;

			Queue<ParsedData> tasks = CommandParser.Parse(Commands, this);

			foreach (var item in tasks) {
				await item.Execute();
			}
		}
	}
}
