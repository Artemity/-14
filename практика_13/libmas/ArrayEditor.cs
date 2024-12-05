
using Microsoft.Win32;
using System.Data;
using System.IO;

namespace libmas
{
    /// <summary>
    /// Класс для работы с массивами. В нем содержаться методы для Сохранения массива, 
    /// Создания и Заполнения случайными числами, Удаления и Открытия из файла.
    /// </summary>
    public class ArrayEditor
    {
        /// <summary>
        /// Создает и инициализирует массив числами от 0 до 50
        /// </summary>
        /// <param name="row"> Количество строк, для создания массива </param>
        /// <param name="column"> Количество полей, для создания массива </param>
        /// <returns> Массив, проинициализированный случайными числами </returns>
        public static double[,] Fill(int row, int column)
        {
            Random rnd = new Random();
            double[,] array = new double[row, column];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] = rnd.NextDouble() * 50; // Генерируем случайные вещественные числа от 0 до 50
                }
            }
            return array;
        }
        /// <summary>
        /// Метод читает из файла разрешения *.txt числовые символы, и записывает их в массив
        /// </summary>
        /// <returns name="array">
        /// Возвращает массив, содержащий символы из прочитанного файла, или null, если в файле есть не числовые символы или 
        /// если пользователь закрыл диалоговое окно.
        /// </returns>
        public static double[,]? Open()
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Все файлы (*.*)|*.*|Текстовые файлы (*.txt)|*.txt";
            open.FilterIndex = 2;
            open.Title = "Открытие таблицы";
            int row = 0;
            int column = 0;
            List<double> values = new List<double>();
            if (open.ShowDialog() == true)
            {
                using (StreamReader file = new StreamReader(open.FileName))
                {
                    while (!file.EndOfStream)
                    {
                        string line = file.ReadLine();
                        string[] valuesStr = line.Split(' ');
                        foreach (string valueStr in valuesStr)
                        {
                            if (double.TryParse(valueStr, out double value))
                            {
                                values.Add(value);
                                column++;
                            }
                            else
                            {
                                return null; // Если есть нечисловые символы
                            }
                        }
                        row++;
                    }
                }
                column /= row; // Пересчитываем столбцы
                double[,] array = new double[row, column];
                for (int i = 0; i < array.GetLength(0); i++)
                {
                    for (int j = 0; j < array.GetLength(1); j++)
                    {
                        array[i, j] = values[i * column + j];
                    }
                }
                return array;
            }
            return null;
        }
        /// <summary>
        /// Метод записывает массив в файл формата *.txt
        /// </summary>
        /// <param name="array">Массив, который необходимо сохранить</param>
        public static void Save(double[,] array)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.DefaultExt = ".txt";
            save.Filter = "Текстовые файлы (*.txt)|*.txt";
            save.Title = "Сохранение таблицы";
            if (save.ShowDialog() == true && array != null)
            {
                using (StreamWriter file = new StreamWriter(save.FileName))
                {
                    for (int i = 0; i < array.GetLength(0); i++)
                    {
                        for (int j = 0; j < array.GetLength(1); j++)
                        {
                            file.Write(array[i, j].ToString("F2")); // Форматируем вещественные числа
                            if (j < array.GetLength(1) - 1)
                            {
                                file.Write(" ");
                            }
                        }
                        file.WriteLine();
                    }
                }
            }
        }
    }
    /// <summary>
    /// Класс для взаимодействия массива и DataGrid, формирует названия полей и заполняет ячейки числами
    /// </summary>
    public static class VisualArray
    {
        public static DataTable ToDataTable<T>(this T[,] matrix)
        {
            var res = new DataTable();
            if (matrix != null)
            {
                for (int i = 0; i < matrix.GetLength(1); i++)
                {
                    res.Columns.Add("" + (i), typeof(T));
                }

                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    var row = res.NewRow();

                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        row[j] = matrix[i, j];
                    }

                    res.Rows.Add(row);
                }
            }
            return res;
        }
    }
}