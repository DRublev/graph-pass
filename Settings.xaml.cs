using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Microsoft.Win32;

namespace courser {
				/// <summary>
				/// Логика взаимодействия для Settings.xaml
				/// </summary>
				public partial class Settings : Window {
								public bool shouldDraw = false;
								private bool isPhotoChoosen = false;
								private string passPath;
								private string imagePath;
								private string imageName;

								ConfigProvider config = ConfigProvider.getInstance();

								public Settings() {
												InitializeComponent();
								}

								private void button_Click(object sender, RoutedEventArgs e) {
												ChoosePhoto();
								}

								private void ChoosePhoto() {
												OpenFileDialog op = new OpenFileDialog();
												op.Title = "Select a picture";
												op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
														"JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
														"Portable Network Graphic (*.png)|*.png";
												if(op.ShowDialog() == true) {
																string fullPath = op.FileName;
																string[] partsFileName = fullPath.Split('\\');
																string filename = partsFileName[partsFileName.Length - 1];
																string destination = Path.Combine(config.ImagesFolder, filename);

																File.Copy(fullPath, destination, true);
																choosenPhotoLabel.Content = filename;
																isPhotoChoosen = true;
																config.BackgroundImg = filename;
												}
								}

								private void _1_jpg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
												_1_jpg.BorderBrush = new SolidColorBrush(Color.FromRgb(110, 220, 181));
												_1_jpg.BorderThickness = new Thickness(2, 2, 2, 2);

												_2_jpg.BorderBrush = Brushes.Transparent;
												_3_jpg.BorderBrush = Brushes.Transparent;
												_4_jpg.BorderBrush = Brushes.Transparent;
												ChooseTemplate("1");
								}

								private void _2_jpg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
												_2_jpg.BorderBrush = new SolidColorBrush(Color.FromRgb(110, 220, 181));
												_2_jpg.BorderThickness = new Thickness(2, 2, 2, 2);

												_1_jpg.BorderBrush = Brushes.Transparent;
												_3_jpg.BorderBrush = Brushes.Transparent;
												_4_jpg.BorderBrush = Brushes.Transparent;

												ChooseTemplate("2");
								}

								private void _3_jpg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
												_3_jpg.BorderBrush = new SolidColorBrush(Color.FromRgb(110, 220, 181));
												_3_jpg.BorderThickness = new Thickness(2, 2, 2, 2);

												_1_jpg.BorderBrush = Brushes.Transparent;
												_2_jpg.BorderBrush = Brushes.Transparent;
												_4_jpg.BorderBrush = Brushes.Transparent;
												ChooseTemplate("3");
								}

								private void _4_jpg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
												_4_jpg.BorderBrush = new SolidColorBrush(Color.FromRgb(110, 220, 181));
												_4_jpg.BorderThickness = new Thickness(2, 2, 2, 2);

												_1_jpg.BorderBrush = Brushes.Transparent;
												_2_jpg.BorderBrush = Brushes.Transparent;
												_3_jpg.BorderBrush = Brushes.Transparent;
												ChooseTemplate("4");
								}

								private void ChooseTemplate(string templateFolder) {
												imageName = $"{templateFolder}.jpg";
												imagePath = Path.Combine(ConfigProvider.ResourcesFolder, "templates", templateFolder, imageName);
												passPath = Path.Combine(ConfigProvider.ResourcesFolder, "templates", templateFolder, "pass.csv");


												string[] passLines = { };
												if(File.Exists(passPath)) {
																passLines = File.ReadAllLines(passPath);
												}

												isPhotoChoosen = true;
												shouldDraw = true;

												config.MaxLines = (byte) (passLines.Length - 1);
								}

								private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
												Regex regex = new Regex("[^0-9]+");
												e.Handled = regex.IsMatch(e.Text);
								}

								private void save_Click(object sender, RoutedEventArgs e) {
												if(!isValid())
																return;

												if(shouldDraw) {
																string passDestination = Path.Combine(ConfigProvider.ResourcesFolder, "saved", "pass.csv");
																string imgDestination = Path.Combine(config.ImagesFolder, imageName);
																File.Copy(passPath, passDestination, true);
																File.Copy(imagePath, imgDestination, true);

																config.BackgroundImg = imageName;
												}

												config.Greeting = greeting.Text;
												config.MaxLines = Convert.ToByte(lines.Text);
												//ChooseSuccessModal modal = new ChooseSuccessModal("Enter password");
												//modal.ShowDialog();
												Close();
								}

								private bool isValid() {
												if(lines.Text.Length == 0) {
																MessageBox.Show("Line counts is required field!");
																return false;
												}
												if(Convert.ToByte(lines.Text) < 5) {
																MessageBox.Show("Line counts must be equal or greater than 5");
																return false;
												}
												if(greeting.Text.Length == 0) {
																MessageBox.Show("Greeting is required field!");
																return false;
												}
												if(!isPhotoChoosen) {
																MessageBox.Show("You should choose photo or temolate");
																return false;
												}

												return true;
								}
				}
}
