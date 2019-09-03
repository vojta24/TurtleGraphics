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

namespace TurtleGraphics {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged {

		/*
move Width,100
move 50,200
move 150,500
move 30,800
move 300,320
move 20,100


for i in 0..50{
move i,50
}

for i in 0..8{
rotate i*45
fwd 40
}



move 20,20
rotate 20
fwd 20
for i in 0..50{
rotate 5
fwd 20
}
*/

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

		public double DrawWidth;
		public double DrawHeight;

		private string _color = "Blue";
		private double _brushSize = 5;
		private Point _startPoint;
		private ICommand _runCommand;
		private string _commands;
		private double _angle;
		private double _x;
		private double _y;
		private int _delay = 5;

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
				LineSegment l = new LineSegment();
				l.IsStroked = true;
				l.Point = new Point(X, Y);
				X = to.X;
				Y = to.Y;
				l.IsSmoothJoin = true;

				Segments.Add(l);

				await Displace(to);
			});
		}


		internal void Rotate(double angle) {
			Angle += Math.PI * angle / 180.0;
			if (Angle == 2 * Math.PI) {
				Angle = 0;
			}
		}


		public async Task Forward(double length) {
			double targetX = X + Math.Cos(Angle) * length;
			double targetY = Y + Math.Sin(Angle) * length;

			await Draw(new Point(targetX, targetY));
		}

		public async Task Displace(Point to) {
			int last = Segments.Count - 1;
			LineSegment l = (LineSegment)Segments[last];
			Point origin = l.Point;
			const double INTERATION = 2;
			const double INCREMENT = 1d / INTERATION;
			double curr = 0;
			for (int i = 0; i <= INTERATION; i++) {
				l.Point = new Point(Lerp(origin.X, to.X, curr), Lerp(origin.Y, to.Y, curr));
				Segments[last] = l;
				await Task.Delay(Delay);
				curr += INCREMENT;
			}
			l.Freeze();
		}

		private async Task RunCommandAction() {
			Segments.Clear();
			X = 600;
			Y = 400;
			StartPoint = new Point(X, Y);
			Angle = 0;

			Queue<ParsedData> tasks = await CommandParser.Parse(Commands, this);

			foreach (var item in tasks) {
				await item.Execute();
			}
		}
	}
}
