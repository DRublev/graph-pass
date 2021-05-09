using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace courser {
				/// <summary>
				/// Логика взаимодействия для MainWindow.xaml
				/// </summary>
				public partial class MainWindow : Window {
								byte leftAttempts = ConfigProvider.AllowedAttempts;

								private int currentLinesCount = 0;
								static string passFilePath = System.IO.Path.Combine(@"./Resources/saved", "pass.csv");
								ConfigProvider config = ConfigProvider.getInstance();

								bool isPassExists = File.Exists(passFilePath);

								private List<string> points = new List<string>();

								public MainWindow() {
												InitializeComponent();

												Closing += OnWindowClosing;

												CanvasComponent.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(CanvasComponent_PreviewMouseLeftButtonDown);
												CanvasComponent.MouseMove += new MouseEventHandler(CanvasComponent_PreviewMouseMove);
												CanvasComponent.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(CanvasComponent_PreviewMouseLeftButtonUp);

												if(!isPassExists) {
																Settings settings = new Settings();
																settings.ShowDialog();
																if(settings.shouldDraw) {
																				string[] lines = File.ReadAllLines(passFilePath);
																				DebugDrawPass(lines);
																				CanvasComponent.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DrawModeExitHandler);
																}
																isPassExists = File.Exists(passFilePath);
												}

												ReadConfig();
												SetBackground();
								}

								public void OnWindowClosing(object sender, CancelEventArgs e) {
												if(!isPassExists) {
																SaveToFile();
												}
								}

								private void SaveToFile() {
												File.WriteAllLines(passFilePath, points.Select(x => string.Join(",", x)));
												isPassExists = true;
								}

								private void ReadConfig() {
												try {
																config = ConfigProvider.getInstance();
												}
												catch(Exception ex) {
																FailModal modal = new FailModal(ex.Message);
																modal.ShowDialog();
																Close();
												}
								}

								private void SetBackground() {
												ImageBrush ib = new ImageBrush();
												ib.ImageSource = new BitmapImage(new Uri(config.BackgroundImg, UriKind.RelativeOrAbsolute));
												CanvasComponent.Background = ib;
								}

								private bool IsInAllowedErrorArea(int value, int toCompare) {
												int greater = value + ConfigProvider.ErrorValue;
												int less = value - ConfigProvider.ErrorValue;
												bool _is = toCompare >= less && toCompare <= greater;
												return _is;
								}

								private void CheckPassword() {
												string[] lines = File.ReadAllLines(passFilePath);

												bool isAimed = points.All((point) => {
																string[] coords = point.Split(ConfigProvider.Separator);
																bool isOneOf = false;
																foreach(string l in lines) {
																				(int x1, int y1, int x2, int y2) = ParseCoords(point);
																				(int cx1, int cy1, int cx2, int cy2) = ParseCoords(l);

																				bool isX1 = IsInAllowedErrorArea(x1, cx1);
																				bool isY1 = IsInAllowedErrorArea(y1, cy1);
																				bool isX2 = IsInAllowedErrorArea(x2, cx2);
																				bool isY2 = IsInAllowedErrorArea(y2, cy2);

																				bool isNear = isX1 && isY1 && isX2 && isY2;

																				isOneOf = isOneOf || isNear;
																}
																return isOneOf;
												});

												if(isAimed) {
																ChooseSuccessModal modal = new ChooseSuccessModal($"Hello {config.Greeting}!", true);
																modal.ShowDialog();

																if(modal.ClearEnteredPassword) {
																				ClearEnteredPassword();
																				File.Delete(passFilePath);
																} else {
																				Close();
																}
												}
												else if(leftAttempts > 1) {
																FailModal modal = new FailModal($"Wrong password! Attempts left: {leftAttempts}");
																modal.ShowDialog();
																leftAttempts -= 1;
																ClearEnteredPassword();
												}
												else {
																FailModal modal = new FailModal("Wrong password! Exiting...");
																modal.ShowDialog();
																Close();
												}
								}

								Line newLine;
								Point clickPoint;
								Point drawPoint;
								private void CanvasComponent_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
												if(currentLinesCount == config.MaxLines) {
																return;
												}

												clickPoint = (Point) e.GetPosition(this);
												newLine = new Line();
												SolidColorBrush lineColor = new SolidColorBrush(Color.FromArgb(166, 255, 255, 255));
												newLine.Stroke = lineColor;
												newLine.Fill = lineColor;
												newLine.StrokeLineJoin = PenLineJoin.Round;
												newLine.StrokeEndLineCap = PenLineCap.Round;
												newLine.StrokeStartLineCap = PenLineCap.Round;

												newLine.X1 = clickPoint.X;
												newLine.Y1 = clickPoint.Y;
												newLine.X2 = clickPoint.X;
												newLine.Y2 = clickPoint.Y;
												newLine.StrokeThickness = 8;
												CanvasComponent.Children.Add(newLine);
												int zindex = CanvasComponent.Children.Count;
												Canvas.SetZIndex(newLine, zindex);

												currentLinesCount = CanvasComponent.Children.OfType<Line>().Where(IsLine).Count();
								}

								private bool IsLine(Line line) {
												Vector2 start = new Vector2((float) line.X1, (float) line.Y1);
												Vector2 end = new Vector2((float) line.X2, (float) line.Y2);
												return Vector2.Distance(start, end) > 0;
								}

								private void CanvasComponent_PreviewMouseMove(object sender, MouseEventArgs e) {
												drawPoint = (Point) e.GetPosition(CanvasComponent);
												if(newLine != null & e.LeftButton == MouseButtonState.Pressed) {
																newLine.X2 = drawPoint.X;
																newLine.Y2 = drawPoint.Y;
												}
								}

								private void CanvasComponent_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
												if(newLine != null && IsLine(newLine)) {
																points.Add($"{newLine.X1}{ConfigProvider.Separator}{newLine.Y1}{ConfigProvider.Separator}{newLine.X2}{ConfigProvider.Separator}{newLine.Y2}");
												}
												if(currentLinesCount == config.MaxLines && isPassExists) {
																CheckPassword();
												}
												else if(currentLinesCount == config.MaxLines) {
																ChooseSuccessModal modal = new ChooseSuccessModal("Enter your password");
																modal.ShowDialog();
																SaveToFile();
																ClearEnteredPassword();
												}


												newLine = null;
								}

								private (int, int, int, int) ParseCoords(string line) {
												string[] lineCoords = line.Split(ConfigProvider.Separator);
												int x1 = Convert.ToInt32(lineCoords[0].Trim());
												int y1 = Convert.ToInt32(lineCoords[1].Trim());
												int x2 = Convert.ToInt32(lineCoords[2].Trim());
												int y2 = Convert.ToInt32(lineCoords[3].Trim());

												return (x1, y1, x2, y2);
								}

								private void ClearEnteredPassword() {
												CanvasComponent.Children.Clear();
												currentLinesCount = 0;
								}

								protected void DebugDrawPass(string[] lines) {
												foreach(string l in lines) {
																(int x1, int y1, int x2, int y2) = ParseCoords(l);

																Line passLine = new Line();
																SolidColorBrush lineColor = new SolidColorBrush(Color.FromRgb(133, 173, 233));
																passLine.Stroke = lineColor;
																passLine.Fill = lineColor;
																passLine.StrokeLineJoin = PenLineJoin.Round;
																passLine.StrokeEndLineCap = PenLineCap.Round;
																passLine.StrokeStartLineCap = PenLineCap.Round;
																passLine.X1 = x1;
																passLine.Y1 = y1;
																passLine.X2 = x2;
																passLine.Y2 = y2;
																passLine.StrokeThickness = 8;
																CanvasComponent.Children.Add(passLine);
												}
								}

								private void DrawModeExitHandler(object sender, MouseButtonEventArgs e) {
												ClearEnteredPassword();

												CanvasComponent.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(DrawModeExitHandler);
								}
				}
}
