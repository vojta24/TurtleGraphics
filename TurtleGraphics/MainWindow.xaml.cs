using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Flee.PublicTypes;
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


		#region Bindings

		private string _color;
		private double _brushSize;
		private Point _startPoint;
		private ICommand _runCommand;
		private string _commandsText = "";
		private double _angle;
		private double _x;
		private double _y;
		private int _delay;
		private bool _penDown;
		private int _iterationCount;
		private ICommand _buttonCommand;
		private ICommand _stopCommand;
		private string _buttonText = "Run";
		private ICommand _toggleFullScreenCommand;
		private bool _toggleFullscreenEnabled = true;
		private string _buttonTextFullSize = "Run on fullsize canvas";
		private ICommand _buttonFullSizeCommand;
		private bool _showTurtleCheckBox = true;
		private string _inteliCommandsText;

		public string InteliCommandsText { get => _inteliCommandsText; set { _inteliCommandsText = value; Notify(nameof(InteliCommandsText)); } }
		public bool ShowTurtleCheckBox { get => _showTurtleCheckBox; set { _showTurtleCheckBox = value; Notify(nameof(ShowTurtleCheckBox)); } }
		public ICommand ButtonFullSizeCommand { get => _buttonFullSizeCommand; set { _buttonFullSizeCommand = value; Notify(nameof(ButtonFullSizeCommand)); } }
		public string ButtonTextFullSize { get => _buttonTextFullSize; set { _buttonTextFullSize = value; Notify(nameof(ButtonTextFullSize)); } }
		public bool ToggleFullscreenEnabled { get => _toggleFullscreenEnabled; set { _toggleFullscreenEnabled = value; Notify(nameof(ToggleFullscreenEnabled)); } }
		public ICommand ToggleFullScreenCommand { get => _toggleFullScreenCommand; set { _toggleFullScreenCommand = value; Notify(nameof(ToggleFullScreenAction)); } }
		public string ButtonText { get => _buttonText; set { _buttonText = value; Notify(nameof(ButtonText)); } }
		public ICommand StopCommand { get => _stopCommand; set { _stopCommand = value; Notify(nameof(StopCommand)); } }
		public ICommand ButtonCommand { get => _buttonCommand; set { _buttonCommand = value; Notify(nameof(ButtonCommand)); } }
		public int IterationCount { get => _iterationCount; set { _iterationCount = value; Notify(nameof(IterationCount)); } }
		public bool PenDown { get => _penDown; set { if (value == _penDown) return; _penDown = value; NewPath(); Notify(nameof(PenDown)); } }
		public int Delay { get => _delay; set { _delay = value; Notify(nameof(Delay)); } }
		public double Y { get => _y; set { _y = value; Notify(nameof(Y)); } }
		public double X { get => _x; set { _x = value; Notify(nameof(X)); } }
		public double Angle { get => _angle; set { _angle = value; Notify(nameof(Angle)); } }
		public string CommandsText { get => _commandsText; set { _commandsText = value; Notify(nameof(CommandsText)); } }
		public ICommand RunCommand { get => _runCommand; set { _runCommand = value; Notify(nameof(RunCommand)); } }
		public Point StartPoint { get => _startPoint; set { _startPoint = value; Notify(nameof(StartPoint)); } }
		public double BrushSize { get => _brushSize; set { if (value == _brushSize) return; _brushSize = value; NewPath(); Notify(nameof(BrushSize)); } }
		public string Color { get => _color; set { if (value == _color) return; _color = value; NewPath(); Notify(nameof(Color)); } }


		#endregion


		private Path _currentPath;
		private PathFigure _currentFigure;
		private CancellationTokenSource cancellationTokenSource;
		private bool _inteliCommandsEnabled = true;
		private InteliCommandsHandler _inteliCommands = new InteliCommandsHandler();


		public double DrawWidth { get; set; }
		public double DrawHeight { get; set; }

		public MainWindow() {
			InitializeComponent();
			RunCommand = new AsyncCommand(RunCommandAction);
			StopCommand = new Command(() => {
				cancellationTokenSource.Cancel();
				ButtonCommand = RunCommand;
				ButtonText = "Run";
			});
			ButtonCommand = RunCommand;
			ToggleFullScreenCommand = new Command(ToggleFullScreenAction);
			ButtonFullSizeCommand = new Command(async () => {
				if (ButtonCommand == RunCommand) {
					ControlArea.Width = new GridLength(0, GridUnitType.Pixel);
					await Task.Delay(1);
					DrawWidth = DrawAreaX.ActualWidth;
					ButtonCommand.Execute(null);
					await Task.Delay(1);
					IterationCount = 1;
					Delay = 1;
					PreviewKeyDown += MainWindow_KeyDown;
				}
			});
			Loaded += MainWindow_Loaded;
			SizeChanged += MainWindow_SizeChanged;
			CommandsTextInput.SelectionChanged += CommandsTextInput_SelectionChanged;
			DataContext = this;
		}

		private void CommandsTextInput_SelectionChanged(object sender, RoutedEventArgs e) {
			Console.WriteLine("Carret: " + CommandsTextInput.CaretIndex);
			Console.WriteLine("SelectionStart: " + CommandsTextInput.SelectionStart);
			Console.WriteLine("Length: " + CommandsTextInput.SelectionLength);
			_inteliCommands.Handle(this, CommandsTextInput);
			Notify(nameof(CommandsText));
			Notify(nameof(InteliCommandsText));
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

			Color = "Blue";
			PenDown = true;
			X = DrawWidth / 2;
			Y = DrawHeight / 2;
			TurtleTranslation.X = X;
			TurtleTranslation.Y = Y;
			TurtleRotation.Angle = 90;
			TurtleScale.ScaleX = 1;
			TurtleScale.ScaleY = 1;
			StartPoint = new Point(X, Y);
			Angle = 0;
			BrushSize = 4;
			IterationCount = 10;
			Delay = 5;

			NewPath();
		}


		#region Events

		private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
			DrawWidth = DrawAreaX.ActualWidth;
			DrawHeight = DrawAreaY.ActualHeight;
			ContextExtensions.SetWindow(this);
			Init();
			Loaded -= MainWindow_Loaded;
		}


		private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e) {
			DrawWidth = DrawAreaX.ActualWidth;
			DrawHeight = DrawAreaY.ActualHeight;
			Init();
		}

		private async void MainWindow_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Escape) {
				ControlArea.Width = new GridLength(1, GridUnitType.Star);
				await Task.Delay(1);
				DrawWidth = DrawAreaX.ActualWidth;
				PreviewKeyDown -= MainWindow_KeyDown;
			}
		}

		#endregion


		public void NewPath() {
			if (_currentPath != null) {
				_currentPath.Data.Freeze();
			}

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
				TurtleRotation.Angle = 90;
				return;
			}
			if (setRotation) {
				Angle = ContextExtensions.AsRad(angle);
				TurtleRotation.Angle = 90 + angle;
			}
			else {
				Angle += ContextExtensions.AsRad(angle);
				TurtleRotation.Angle += angle;
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

		#region Drawing lines

		public async Task Draw(Point to) {
			_currentFigure.Segments.Add(new LineSegment(new Point(X, Y), true) { IsSmoothJoin = true });
			X = to.X;
			Y = to.Y;
			await Displace(to);
		}


		public async Task Displace(Point to) {
			int last = _currentFigure.Segments.Count - 1;
			LineSegment lastSegment = (LineSegment)_currentFigure.Segments[last];
			Point origin = lastSegment.Point;
			double increment = 1d / IterationCount;
			double currentInterpolation = 0;

			for (int i = 0; i <= IterationCount; i++) {
				if (cancellationTokenSource.Token.IsCancellationRequested) {
					break;
				}
				lastSegment.Point = new Point(Lerp(origin.X, to.X, currentInterpolation), Lerp(origin.Y, to.Y, currentInterpolation));
				TurtleTranslation.X = lastSegment.Point.X;
				TurtleTranslation.Y = lastSegment.Point.Y;
				_currentFigure.Segments[last] = lastSegment;
				currentInterpolation += increment;
				await Task.Delay(Delay);
			}
			lastSegment.Freeze();
		}

		#endregion


		#region Actions

		private async Task RunCommandAction() {
			ToggleFullscreenEnabled = false;
			Init();
			cancellationTokenSource = new CancellationTokenSource();
			ButtonCommand = StopCommand;
			ButtonText = "Stop";
			Queue<ParsedData> tasks = CommandParser.Parse(CommandsText, this);

			foreach (var item in tasks) {
				await item.Execute(cancellationTokenSource.Token);
				if (cancellationTokenSource.Token.IsCancellationRequested) {
					break;
				}
			}
			ButtonCommand = RunCommand;
			ButtonText = "Run";
			ToggleFullscreenEnabled = true;
		}


		public void ToggleFullScreenAction() {
			if (WindowStyle == WindowStyle.SingleBorderWindow) {
				WindowStyle = WindowStyle.None;
				WindowState = WindowState.Maximized;
			}
			else {
				WindowStyle = WindowStyle.SingleBorderWindow;
				WindowState = WindowState.Normal;
			}
		}

		#endregion
	}
}
