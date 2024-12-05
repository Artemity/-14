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
using System.IO; 


namespace практика_13
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public int Rows { get; private set; }
        public int Cols { get; private set; }
        public Settings()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Кнопка сохранения настроек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(tbRows.Text, out int rows) && int.TryParse(tbCols.Text, out int cols))
            {
                this.Rows = rows;
                this.Cols = cols;

                File.WriteAllText("config.ini", $"{rows},{cols}");

                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Введите корректные значения.", "Ошибка");
            }
        }
    }
}
