using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace courser {
				/// <summary>
				/// Логика взаимодействия для ChooseSuccessModal.xaml
				/// </summary>
				public partial class ChooseSuccessModal : Window {
								public bool ClearEnteredPassword = false;

								public ChooseSuccessModal(string message = "", bool showDeleteBtn = false) {
												InitializeComponent();
												messageBlock.Text = message;
												deleteBtn.Visibility = showDeleteBtn ? Visibility.Visible : Visibility.Hidden;
								}

								private void button_Click(object sender, RoutedEventArgs e) {
												Close();
								}

								private void deleteBtn_Click(object sender, RoutedEventArgs e) {
												ClearEnteredPassword = true;
												Close();
								}
				}
}
