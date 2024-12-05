
using Microsoft.Win32;
using System.Data;
using System.IO;

namespace libmas
{
    /// <summary>
    /// ����� ��� ������ � ���������. � ��� ����������� ������ ��� ���������� �������, 
    /// �������� � ���������� ���������� �������, �������� � �������� �� �����.
    /// </summary>
    public class ArrayEditor
    {
        /// <summary>
        /// ������� � �������������� ������ ������� �� 0 �� 50
        /// </summary>
        /// <param name="row"> ���������� �����, ��� �������� ������� </param>
        /// <param name="column"> ���������� �����, ��� �������� ������� </param>
        /// <returns> ������, ��������������������� ���������� ������� </returns>
        public static double[,] Fill(int row, int column)
        {
            Random rnd = new Random();
            double[,] array = new double[row, column];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] = rnd.NextDouble() * 50; // ���������� ��������� ������������ ����� �� 0 �� 50
                }
            }
            return array;
        }
        /// <summary>
        /// ����� ������ �� ����� ���������� *.txt �������� �������, � ���������� �� � ������
        /// </summary>
        /// <returns name="array">
        /// ���������� ������, ���������� ������� �� ������������ �����, ��� null, ���� � ����� ���� �� �������� ������� ��� 
        /// ���� ������������ ������ ���������� ����.
        /// </returns>
        public static double[,]? Open()
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "��� ����� (*.*)|*.*|��������� ����� (*.txt)|*.txt";
            open.FilterIndex = 2;
            open.Title = "�������� �������";
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
                                return null; // ���� ���� ���������� �������
                            }
                        }
                        row++;
                    }
                }
                column /= row; // ������������� �������
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
        /// ����� ���������� ������ � ���� ������� *.txt
        /// </summary>
        /// <param name="array">������, ������� ���������� ���������</param>
        public static void Save(double[,] array)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.DefaultExt = ".txt";
            save.Filter = "��������� ����� (*.txt)|*.txt";
            save.Title = "���������� �������";
            if (save.ShowDialog() == true && array != null)
            {
                using (StreamWriter file = new StreamWriter(save.FileName))
                {
                    for (int i = 0; i < array.GetLength(0); i++)
                    {
                        for (int j = 0; j < array.GetLength(1); j++)
                        {
                            file.Write(array[i, j].ToString("F2")); // ����������� ������������ �����
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
    /// ����� ��� �������������� ������� � DataGrid, ��������� �������� ����� � ��������� ������ �������
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