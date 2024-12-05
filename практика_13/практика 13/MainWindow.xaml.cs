using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using libmas;
using System.IO; // Импорт для работы с файловой системой


namespace практика_13
{
    public partial class MainWindow : Window
    {
        private double[,] _matrixInitial; // Исходная вещественная матрица
        private double[,] _matrixResult;  // Вещественная матрица результат

        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
        }

        /// <summary>
        /// Обработчик событий Window_Loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Password pas = new Password();
            pas.Owner = this;
            if (pas.ShowDialog() != true)
            {
                this.Close();
            }
        }

        /// <summary>
        /// Кнопка открытия настроек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            Settings settingsWindow = new Settings();
            if (settingsWindow.ShowDialog() == true)
            {
                int rows = settingsWindow.Rows;
                int cols = settingsWindow.Cols;

                rowText.Text = rows.ToString();
                columnText.Text = cols.ToString();
            }
        }

        /// <summary>
        /// Метод загрузки конфига
        /// </summary>
        private void LoadSettings()
        {
            if (File.Exists("config.ini"))
            {
                var content = File.ReadAllText("config.ini").Split(',');
                if (int.TryParse(content[0], out int rows) && int.TryParse(content[1], out int cols))
                {
                    rowText.Text = rows.ToString();
                    columnText.Text = cols.ToString();
                }
            }
        }

        /// <summary>
        /// Событие вывода информации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Программу подготовил студент группы ИСП-31 Лотаков Артемий\nПрактическая 14 Вариант 13\nДана вещественная матрица A(M, N). Строку, содержащий максимальный элемент, поменять местами со строкой, содержащей минимальный элемент.", "О программе");
        }

        /// <summary>
        /// Кнопка выхода
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Кнопка сохранения таблицы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_matrixInitial == null)
            {
                MessageBox.Show("Сначала создайте таблицу");
                return;
            }
            ArrayEditor.Save(_matrixInitial); // Используем метод сохранения из ArrayEditor для исходной вещественной матрицы
        }

        /// <summary>
        /// Кнопка открытия таблицы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            _matrixInitial = ArrayEditor.Open(); // Используем метод открытия из ArrayEditor для вещественной матрицы
            if (_matrixInitial != null)
            {
                rowText.Text = _matrixInitial.GetLength(0).ToString(); // Устанавливаем текстовое поле для количества строк в матрице
                columnText.Text = _matrixInitial.GetLength(1).ToString();  // Устанавливаем текстовое поле для количества столбцов в матрице
                initialTable.ItemsSource = VisualArray.ToDataTable(_matrixInitial).DefaultView; // Преобразуем загруженную матрицу в DataTable и устанавливаем его в качестве источника данных для элемента управления initialTable
                size.Text = string.Format("Размер таблицы: {0}х{1}", _matrixInitial.GetLength(0), _matrixInitial.GetLength(1)); // Обновляем текстовое поле размером таблицы, отображая количество строк и столбцов
            }
        }

        /// <summary>
        /// Обработчик для кнопки очистки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            rowText.Clear();
            columnText.Clear();
            initialTable.ItemsSource = null;
            resultTable.ItemsSource = null;
            _matrixInitial = null;
            _matrixResult = null;
            size.Text = string.Format("Размер таблицы: 0х0");
            selectedText.Text = string.Format("Выбранная ячейка: 0х0");
        }

        /// <summary>
        /// Обработчик для кнопки создания и заполнения матрицы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShow_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(rowText.Text, out int row) && int.TryParse(columnText.Text, out int column) && row > 0 && column > 0)
            {
                _matrixInitial = ArrayEditor.Fill(row, column); // Используем метод заполнения из ArrayEditor для вещественной матрицы
                initialTable.ItemsSource = VisualArray.ToDataTable(_matrixInitial).DefaultView; // Преобразуем загруженную матрицу в DataTable и устанавливаем его в качестве источника данных для элемента управления initialTable
                resultTable.ItemsSource = null;
                size.Text = string.Format("Размер таблицы: {0}х{1}", row, column);
            }
            else
            {
                MessageBox.Show("Введены неверные данные");
            }
        }

        /// <summary>
        /// Обработчик для кнопки вычисления.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (_matrixInitial == null)
            {
                MessageBox.Show("Сначала создайте таблицу");
                return;
            }

            // Создаем новую матрицу _matrixResult с такими же размерами, как и у _matrixInitial.
            _matrixResult = new double[_matrixInitial.GetLength(0), _matrixInitial.GetLength(1)];

            // Копируем значения из _matrixInitial в _matrixResult.
            for (int i = 0; i < _matrixInitial.GetLength(0); i++) // Перебираем все строки.
            {
                for (int j = 0; j < _matrixInitial.GetLength(1); j++) // Перебираем все столбцы.
                {
                    _matrixResult[i, j] = _matrixInitial[i, j]; // Копируем значение из исходной матрицы.
                }
            }

            // Создаем массив для хранения значений одной строки (для замены).
            double[] array = new double[_matrixResult.GetLength(1)]; // Размер массива равен количеству столбцов.

            int rowMin = 0; // Индекс строки с минимальным элементом.
            int rowMax = 0; // Индекс строки с максимальным элементом.
            double min = _matrixResult[0, 0]; // Инициализируем переменную min значением первого элемента.
            double max = _matrixResult[0, 0]; // Инициализируем переменную max значением первого элемента.

            // Ищем строки с минимальным и максимальным значениями в матрице.
            for (int i = 0; i < _matrixResult.GetLength(0); i++) // Перебираем все строки.
            {
                for (int j = 0; j < _matrixResult.GetLength(1); j++) // Перебираем все столбцы.
                {
                    // Проверяем, меньше ли текущий элемент min.
                    if (_matrixResult[i, j] < min)
                    {
                        min = _matrixResult[i, j]; // Обновляем значение min.
                        rowMin = i; // Сохраняем индекс строки с минимальным значением.
                    }
                    // Проверяем, больше ли текущий элемент max.
                    if (_matrixResult[i, j] > max)
                    {
                        max = _matrixResult[i, j]; // Обновляем значение max.
                        rowMax = i; // Сохраняем индекс строки с максимальным значением.
                    }
                }
            }

            // Меняем местами строки с минимальным и максимальным значениями.
            for (int j = 0; j < _matrixResult.GetLength(1); j++) // Перебираем колонки для строки с минимальным значением.
            {
                array[j] = _matrixResult[rowMin, j]; // Сохраняем значения строки с минимальным элементом во временном массиве.
                _matrixResult[rowMin, j] = _matrixResult[rowMax, j]; // Заменяем строку с минимальным значением на строку с максимальным значением.
            }

            // Теперь восстанавливаем значения строки с максимальным элементом.
            for (int j = 0; j < _matrixResult.GetLength(1); j++) // Перебираем колонки для строки с максимальным значением.
            {
                _matrixResult[rowMax, j] = array[j]; // Восстанавливаем значения строки с максимальным элементом.
            }

            // Обновляем источник данных для результата в таблице, преобразуя матрицу в DataTable.
            resultTable.ItemsSource = VisualArray.ToDataTable(_matrixResult).DefaultView;

        }

        /// <summary>
        /// Обработчик события для изменения выделённых ячеек в таблице результат.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResultTable_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (resultTable.CurrentColumn == null) return;
        }

        /// <summary>
        /// Обработчик события для изменения выделённых ячеек в исходной таблице.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitialTable_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (initialTable.CurrentColumn == null) return;
        }

        /// <summary>
        /// Изменение DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            int indexColumn = e.Column.DisplayIndex;
            int indexRow = e.Row.GetIndex();
            if (!double.TryParse(((TextBox)e.EditingElement).Text.Replace('.', ','), out double value))
            {
                MessageBox.Show("Введены неверные данные");
                return;
            }
            _matrixInitial[indexRow, indexColumn] = value;
            resultTable.ItemsSource = null;
        }

        /// <summary>
        /// Обработчик события изменения текста в текстовых полях.
        /// Очищает содержимое таблиц данных при изменении текста.
        /// </summary>
        private void Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            initialTable.ItemsSource = null;
            resultTable.ItemsSource = null;
        }
    }
}