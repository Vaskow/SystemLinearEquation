using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SystemLineEquations
{
    public partial class Form1 : Form
    {
        double[][] SysLinEq; //система линейных уравнений
        double[][] SysTi; //матрица при умножении 2-х матриц
        double[][] T1; //транспонированная матрица от SysTi для решения 2-х систем
        int indJ; // столбцы
        int indI; //строки
        int lastInd = 0; //предыдущий индекс столбца, где мы ставим заголовок "b"
        bool DelIn0 = false; //показывает, что произошло деление на 0

        bool[] ButtonDown; //запоминает, что кнопки были нажаты
        double[] solutMetod1; //решение 1 методом
        double[] solutMetod2; //решение 2 методом
        double[] solutMetod3; //решение 3 методом
        double[] solutMetod4; //решение 4 методом
        bool ComprSymmetrMatr = false; //матрица симметрична для сравнения методов
        bool MinusUndRoot = false; //минус под корнем для 1 метода

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            indI = int.Parse(textBox1.Text);
            if (indI == 1) { MessageBox.Show("Введите 2 и более количество неизвестных"); }
            indJ = indI + 1;
            SysLinEq = new double[indI][];  //объявили строки
            ButtonDown = new bool[4];  //объявили массив состояний 

            solutMetod1 = new double[indI]; //объявим решения методов, которые пойдут на оценку лучшего из них
            solutMetod2 = new double[indI];
            solutMetod3 = new double[indI];
            solutMetod4 = new double[indI];

            for (int i = 0; i < 4; i++)
            {
                ButtonDown[i] = false; //все кнопки изначально не нажаты
            }


            if (lastInd != 0) { dataGridView1.Columns[lastInd].HeaderText = " "; } //обнуляем заголовок предыдущего столбца

            for (int i = 0; i < indI; i++)
            {
                SysLinEq[i] = new double[indJ]; //объявили столбцы
                for (int j = 0; j < indJ; j++)
                {
                    SysLinEq[i][j] = 0;
                }
            }
            //Создаем таблицу
            dataGridView1.RowCount = indI;
            dataGridView1.ColumnCount = indJ;
            for (int i = 0; i < indJ; i++)
            {
                dataGridView1.Columns[i].Width = 30; //стандартные размеры 
            }
            for (int i = 0; i < indI; i++)
            {
                dataGridView1.Rows[i].Height = 20;
            }

            //Зададим автоподстройку, если значения большие
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            dataGridView1.Columns[indI].HeaderText = "b";
            lastInd = indI; //запоминаем индекс, чтобы обнулить столбец при создании новой матрицы

        }

        private void button2_Click(object sender, EventArgs e) //Метод квадратного корня
        {
            double[] masYi = new double[indJ]; //решение системы T1 = b (Ly = b)
            double[] masXi = new double[indJ]; //решение системы SysTi = Yi (Ux = y)
            bool SymmetricMatrix = true; //матрица СЛУ симметрична
            ButtonDown[0] = true;

            //Объявляем матрицу А (верхнетреугольная) из элементов ti
            SysTi = new double[indI][];  //объявили строки

            for (int i = 0; i < indI; i++)
            {
                SysTi[i] = new double[indJ - 1]; //объявили столбцы
                for (int j = 0; j < indJ - 1; j++)
                {
                    SysTi[i][j] = 0;
                }
            }
            //Объявляем матрицу T1 - транспонированная (нижнетреугольная) А
            T1 = new double[indI][];  //объявили строки

            for (int i = 0; i < indI; i++)
            {
                T1[i] = new double[indJ - 1]; //объявили столбцы
                for (int j = 0; j < indJ - 1; j++)
                {
                    T1[i][j] = 0;
                }
            }

            for (int i = 0; i < indI; i++)
            {
                for (int j = 0; j < indJ; j++)
                {
                    SysLinEq[i][j] = Convert.ToDouble(dataGridView1[j, i].Value); //построили матрицу из таблицы
                }
            }

            //Проверим, что матрица является симметричной
            for (int i = 0; i < indI; i++)
            {
                for (int j = 0; j < indI; j++)
                {
                    if (SysLinEq[i][j] != SysLinEq[j][i] && i != j) { SymmetricMatrix = false; i = indI-1; j = indI-1; }
                    //если матрица не симметрична, то завершаем проверку и не выполняем оставшийся блок кода
                }
            }
            
            if (SymmetricMatrix == true) //Матрица симметрична => можно использовать метод
            {

                for (int i = 0; i < indI; i++) //вывод матрицы
                {
                    for (int j = 0; j < indJ; j++)
                    {
                        Console.Write(SysLinEq[i][j] + " ");
                    }
                    Console.WriteLine();
                }

                //Считаем Ti
                SysTi[0][0] = Math.Sqrt(SysLinEq[0][0]);
                double SumTki = 0;
                double SumTkiTkj = 0;
                bool[] Minus = new bool[indI]; //чтобы не терять минус от диагональных элеметов, если он есть

                for (int i = 0; i < indI; i++) { Minus[i] = false; }
                if(SysLinEq[0][0] < 0) { MinusUndRoot = true; } //есть минус под корнем

                for (int i = 0; i < indI; i++)
                {
                    SumTki = 0;

                    for (int j = 0; j < indI; j++)
                    {
                        
                        if (i == 0 && j > 0)
                        {
                           SysTi[0][j] = SysLinEq[0][j] / SysTi[0][0];
                        } //для 1 строки
                        if (i == j && i > 0 && j > 0) //для диагонали
                        {
                            SumTki = 0;
                            for (int k = 0; k < i; k++)
                            {
                               SumTki += Math.Pow(SysTi[k][i], 2);
                            }
                            if (SysLinEq[i][j] - SumTki < 0) { Minus[i] = true; }

                            SysTi[i][i] = Math.Sqrt(Math.Abs(SysLinEq[i][j] - SumTki));
                            
                        }
                        if (i < j && i != 0) //для элементов дальше диагонали
                        {
                            SumTkiTkj = 0;
                            for (int k = 0; k < i; k++)
                            {
                              SumTkiTkj += SysTi[k][i] * SysTi[k][j];
                            }
                             
                            if (Minus[i] == true)
                            {
                               double StiDiag = -SysTi[i][i];
                               SysTi[i][j] = (SysLinEq[i][j] - SumTkiTkj) / StiDiag;
                            }
                            else
                            {
                               SysTi[i][j] = (SysLinEq[i][j] - SumTkiTkj) / SysTi[i][i];
                            }
                            
                        }

                    }
                }

                //if (DelIn0 == false) //если деления на 0 не было
               // {
                    for (int i = 0; i < indI; i++) //вывод матрицы SysTi
                    {
                        for (int j = 0; j < indJ - 1; j++)
                        {
                            Console.Write(SysTi[i][j] + " ");
                        }
                        Console.WriteLine();
                    }
                    //Транспонируем матрицу SysTi (в T1)
                    for (int i = 0; i < indI; i++) //вывод матрицы
                    {
                        for (int j = 0; j < indJ - 1; j++)
                        {
                            T1[i][j] = SysTi[j][i];
                        }
                    }

                    for (int i = 0; i < indI; i++) //вывод матрицы T1
                    {
                        for (int j = 0; j < indJ - 1; j++)
                        {
                            Console.Write(T1[i][j] + " ");
                        }
                        Console.WriteLine();
                    }
                //}

                //Считаем Yi
                double SumYi = 0; //сумма известных Yi для нахождения неизвестного Yi
                for (int i = 0; i < indI; i++)
                {
                   /// if ((T1[0][0] == 0 || T1[i][i] == 0) && DelIn0 == false) { MessageBox.Show("Случилось деление на 0, введите значения"); DelIn0 = true; }
                    //else if (DelIn0 == false)//проверка деления на 0
                    //{
                        if (i == 0) { masYi[i] = SysLinEq[0][indJ - 1] / T1[0][0]; }
                        else
                        {
                            SumYi = 0;
                            for (int j = 0; j < i; j++)
                            {
                                SumYi += T1[i][j] * masYi[j];
                            }
                            masYi[i] = (SysLinEq[i][indJ - 1] - SumYi) / T1[i][i];
                        }
                    //}
                }
               // if (DelIn0 == false) //деления на 0 не произошло
                ///{
                    for (int j = 0; j < indJ - 1; j++)
                    {
                        Console.Write(masYi[j] + " "); //вывод Yi
                    }
                    Console.WriteLine();

                    //Считаем Xi
                    double SumXi = 0; //сумма известных Xi для нахождения неизвестного Xi
                    for (int i = indI - 1; i >= 0; i--)
                    {
                        if (i == indI - 1) { masXi[i] = masYi[indI - 1] / SysTi[i][i]; /*Console.Write(masYi[indJ - 1] + " ! " + SysTi[i][i]);*/ }
                        else
                        {
                            SumXi = 0;
                            for (int j = indI - 1; j > i; j--)
                            {
                                SumXi += SysTi[i][j] * masXi[j];
                            }
                            masXi[i] = (masYi[i] - SumXi) / SysTi[i][i];
                        }
                    }
                    for (int j = 0; j < indJ - 1; j++)
                    {
                        Console.Write(masXi[j] + " "); //вывод Xi   - Решение метода
                    }
                    Console.WriteLine();

                    //Вводим решение метода
                    label2.Text = "";
                    for (int j = 0; j < indJ - 1; j++)
                    {
                      if (MinusUndRoot == false)
                      {
                        label2.Text += " X" + (j + 1) + " = " + Math.Round(masXi[j], 3) + " ";
                      }
                      else
                      {
                        label2.Text = " Сработало исключение, отрицательное значение под корнем";
                      }

                    }
                    for (int i = 0; i < indI; i++) //запомним решение для сравнения с отсальными
                    {
                      if (MinusUndRoot == false)
                      {
                        solutMetod1[i] = masXi[i];
                        ComprSymmetrMatr = true;
                      }
                      else
                      {
                        solutMetod1[i] = 0;
                        ComprSymmetrMatr = true;
                      }
                    }
                
            }
            else
            {
                //Если матрица несимметричная
                label2.Text = "";
                label2.Text = "Решение не найдено, так как матрица не является симметричной";

                for (int i = 0; i < indI; i++)
                {
                    solutMetod1[i] = 0;
                }
            }

        }
        //-----------
        private void button3_Click(object sender, EventArgs e) //считаем методом LU-разложения
        {
            double[] masYi = new double[indJ]; //решение системы T1 = b
            double[] masXi = new double[indJ]; //решение системы SysTi = Yi
            ButtonDown[1] = true;

            //Из SysTi сделаем Ui
            SysTi = new double[indI][];  //объявили строки

            for (int i = 0; i < indI; i++)
            {
                SysTi[i] = new double[indJ - 1]; //объявили столбцы
                for (int j = 0; j < indJ - 1; j++)
                {
                    SysTi[i][j] = 0;
                }
            }

            //Объявляем матрицу T1 (нижнетреугольная) Li
            T1 = new double[indI][];  //объявили строки

            for (int i = 0; i < indI; i++)
            {
                T1[i] = new double[indJ - 1]; //объявили столбцы
                for (int j = 0; j < indJ - 1; j++)
                {
                    T1[i][j] = 0;
                }
            }

            double[][] Multipl = new double[indI][]; //произведение для проверки L*U=A

            for (int i = 0; i < indI; i++)
            {
                Multipl[i] = new double[indJ - 1]; //объявили столбцы
                for (int j = 0; j < indJ - 1; j++)
                {
                    Multipl[i][j] = 0;
                }
            }


            for (int i = 0; i < indI; i++)
            {
                for (int j = 0; j < indJ; j++)
                {
                    SysLinEq[i][j] = Convert.ToDouble(dataGridView1[j, i].Value); //построили матрицу из таблицы
                    //Матрица А
                }
            }
            for (int i = 0; i < indI; i++) //вывод матрицы
            {
                for (int j = 0; j < indJ; j++)
                {
                    Console.Write(SysLinEq[i][j] + " ");
                }
                Console.WriteLine();
            }
            //Скопируем A в будущую Ui
            for (int i = 0; i < indI; i++)
            {
                for (int j = 0; j < indI; j++)
                {
                    SysTi[i][j] = SysLinEq[i][j];
                    //Матрица Ui = A
                }
            }

            //Посчитаем матрицы Li и Ui
            for (int i = 0; i < indI; i++)
            {
                for (int j = i; j < indI; j++)
                {
                    T1[j][i] = SysTi[j][i] / SysTi[i][i];
                }
            }
            for (int k = 1; k < indI; k++)
            {
                for (int i = k - 1; i < indI; i++)
                {
                    for (int j = i; j < indI; j++)
                    {
                        T1[j][i] = SysTi[j][i] / SysTi[i][i];
                    }
                }
                for (int i = k; i < indI; i++)
                {
                    for (int j = k - 1; j < indI; j++)
                    {
                        SysTi[i][j] = SysTi[i][j] - T1[i][k - 1] * SysTi[k - 1][j];
                    }
                }
            }

            for (int i = 0; i < indI; i++) //вывод матрицы SysTi, т.е. U
            {
                for (int j = 0; j < indJ-1; j++)
                {
                    Console.Write(SysTi[i][j] + " ");
                }
                Console.WriteLine();
            }
            for (int i = 0; i < indI; i++) //вывод матрицы T1, т.е. L
            {
                for (int j = 0; j < indJ-1; j++)
                {
                    Console.Write(T1[i][j] + " ");
                }
                Console.WriteLine();
            }

            //Посчитаем произведение L*U=A
            for (int i = 0; i < indI; i++)
                for (int j = 0; j < indI; j++)
                    for (int k = 0; k < indI; k++)
                        Multipl[i][j] += T1[i][k] * SysTi[k][j];

            for (int i = 0; i < indI; i++) //вывод матрицы A = L * U
            {
                for (int j = 0; j < indJ - 1; j++)
                {
                    Console.Write(Multipl[i][j] + " ");
                }
                Console.WriteLine();
            }

            //Считаем Yi
            double SumYi = 0; //сумма известных Yi для нахождения неизвестного Yi
            for (int i = 0; i < indI; i++)
            {
                if (i == 0) { masYi[i] = SysLinEq[0][indJ - 1] / T1[0][0]; }
                else
                {
                    SumYi = 0;
                    for (int j = 0; j < i; j++)
                    {
                        SumYi += T1[i][j] * masYi[j];
                    }
                    masYi[i] = (SysLinEq[i][indJ - 1] - SumYi) / T1[i][i];
                }
            }
            for (int j = 0; j < indJ - 1; j++)
            {
                Console.Write(masYi[j] + " "); //вывод Yi
            }
            Console.WriteLine();

            //Считаем Xi
            double SumXi = 0; //сумма известных Xi для нахождения неизвестного Xi
            for (int i = indI - 1; i >= 0; i--)
            {
                if (i == indI - 1) { masXi[i] = masYi[indI - 1] / SysTi[i][i]; /*Console.Write(masYi[indJ - 1] + " ! " + SysTi[i][i]);*/ }
                else
                {
                    SumXi = 0;
                    for (int j = indI - 1; j > i; j--)
                    {
                        SumXi += SysTi[i][j] * masXi[j];
                    }
                    masXi[i] = (masYi[i] - SumXi) / SysTi[i][i];
                }
            }
            for (int j = 0; j < indJ - 1; j++)
            {
                Console.Write(masXi[j] + " "); //вывод Xi   - Решение метода
            }
            Console.WriteLine();

            //Выводим решение метода
            label3.Text = "";
            for (int j = 0; j < indJ - 1; j++)
            {
                label3.Text += " X" + (j+1) + " = " + Math.Round(masXi[j], 4) + " ";
            }
            for (int i = 0; i < indI; i++) //запомнили решение для сравнения
            {
                solutMetod2[i] = masXi[i];
            }

        }

        //-----------
        private void button4_Click(object sender, EventArgs e) //считаем методом простых итераций
        {
            //Из матрицы А сделаем SysTi вида C*x + d, C-матрица, d, х - векторы  
            SysTi = new double[indI][];  //объявили строки
            double[] Xi = new double[indI]; //решение СЛУ
            ButtonDown[2] = true;

            for (int i = 0; i < indI; i++)
            {
                SysTi[i] = new double[indJ]; //объявили столбцы
                for (int j = 0; j < indJ; j++)
                {
                    SysTi[i][j] = 0;
                }
            }

            for (int i = 0; i < indI; i++)
            {
                for (int j = 0; j < indJ; j++)
                {
                    SysLinEq[i][j] = Convert.ToDouble(dataGridView1[j, i].Value); //построили матрицу из таблицы
                    //Матрица А
                }
            }
            for (int i = 0; i < indI; i++) //вывод матрицы
            {
                for (int j = 0; j < indJ; j++)
                {
                    Console.Write(SysLinEq[i][j] + " ");
                }
                Console.WriteLine();
            }

            //Проверка на диагональное преобладание
            double SummaElemStrok = 0;
            bool NoDiagonPreoblad = false; //нет диагонального преобладания
            for (int i = 0; i < indI; i++)
            {
                SummaElemStrok = 0;
                for (int j = 0; j < indI; j++)
                {
                    if (i != j) { SummaElemStrok += Math.Abs(SysLinEq[i][j]); }
                }
                if (Math.Abs(SysLinEq[i][i]) < SummaElemStrok) { NoDiagonPreoblad = true; i = indI; }
            }

            if (NoDiagonPreoblad == true) //добавим диагональное преобладание для нашей матрицы
            {
                double[][] MatrA = new double[indI][];  //объявили строки
                double[][] MatrAT = new double[indI][];
                double[] VectB = new double[indI]; //вектор b матрицы SysLiqEq

                for (int i = 0; i < indI; i++)
                {
                    MatrA[i] = new double[indJ]; //объявили столбцы
                    MatrAT[i] = new double[indJ];
                    for (int j = 0; j < indI; j++)
                    {
                        MatrA[i][j] = SysLinEq[i][j];   //матрица А
                        MatrAT[i][j] = SysLinEq[j][i];  //матрица А транспонированная
                    }
                }

                for (int i = 0; i < indI; i++)
                {
                    for (int j = 0; j < indJ; j++)
                    {
                        Console.Write(MatrA[i][j] + " ");
                    }
                    Console.WriteLine();
                }
                for (int i = 0; i < indI; i++)
                {
                    for (int j = 0; j < indJ; j++)
                    {
                        Console.Write(MatrAT[i][j] + " ");
                    }
                    Console.WriteLine();
                }

                for (int i = 0; i < indI; i++)
                {
                    VectB[i] = SysLinEq[i][indJ - 1]; //скопируем вектор b 
                }

                for (int i = 0; i < indI; i++)
                {
                   Console.Write(VectB[i] + " ");
                }
                Console.WriteLine();

                for (int i = 0; i < indI; i++) //перемножим матрицы А и АТ
                {
                    for (int j = 0; j < indJ; j++)
                    {
                        SysLinEq[i][j] = 0;
                    }
                }

                for (int i = 0; i < indI; i++) //перемножим матрицы АТ и А  
                {
                    for (int k = 0; k < indI; k++)
                    {
                        for (int j = 0; j < indI; j++)
                        {
                            SysLinEq[i][k] += MatrAT[i][j] * MatrA[j][k];  // SysLiqEq = AT * A
                        }
                    }

                        for (int j = 0; j < indI; j++)
                        {
                            SysLinEq[i][indJ - 1] += MatrAT[i][j] * VectB[j]; //вектор b = AT * предыд. b
                        }
                }

            }
            for (int i = 0; i < indI; i++) 
            {
                for (int j = 0; j < indJ; j++)
                {
                    Console.Write(SysLinEq[i][j] + " ");
                }
                Console.WriteLine();
            }


            //Скопируем A в SysTi
            for (int i = 0; i < indI; i++)
            {
                for (int j = 0; j < indJ; j++)
                {
                    SysTi[i][j] = SysLinEq[i][j];
                    //Потом приведем к виду Cx+d
                }
            }
            ////Преобразуем к виду Cx+d
            //for (int i = 0; i < indI; i++)
            //{
            //    for (int j = 0; j < indI; j++)
            //    {
            //        //переносим левую часть в правую, затем прибавим к обоим частям xi (i - элемента главной диагонали), с 0 ничего не делаем
            //        if (i == j && SysTi[i][j] != 0) { SysTi[i][j] = SysTi[i][j] * (-1); SysTi[i][j] = SysTi[i][j] + 1; }
            //        else { SysTi[i][j] = SysTi[i][j] * (-1); }
            //    }
            //}

            //Преобразуем к виду Cx+d
            for (int i = 0; i < indI; i++)
            {
                for (int j = 0; j < indI; j++)
                {
                    //переносим левую часть в правую, затем прибавим к обоим частям xi (i - элемента главной диагонали), с 0 ничего не делаем
                    if (i == j) { /*SysTi[i][j] = SysTi[i][j] * (-1); SysTi[i][j] = SysTi[i][j] + 1; */}
                    else { SysTi[i][j] = SysTi[i][j] * (-1); }
                }
            }

            //разделим все элементы строки на диагональные
            double DiagTi = 1;
            for (int i = 0; i < indI; i++) //вывод матрицы SysTi вида Cx+d
            {
                if (SysTi[i][i] != 0)
                {
                    DiagTi = SysTi[i][i]; //запомнили диагональный элемент перед занулением
                }
                else { DiagTi = 1; }
                for (int j = 0; j < indJ; j++)
                {
                    if (i == j)
                    {
                        SysTi[i][j] = 0;
                    }
                    else { SysTi[i][j] = SysTi[i][j] / DiagTi; }
                }

            }


            for (int i = 0; i < indI; i++) //вывод матрицы SysTi вида Cx+d
            {
                for (int j = 0; j < indJ; j++)
                {
                    Console.Write(SysTi[i][j] + " ");
                }
                Console.WriteLine();
            }

            //посчитаем норму матрицы С, она должна быть < 1
            double[] sumCiStrok = new double[indI]; //сумма модулей элементов строк матрицы C
            double maxSum; //максимальная из предыдущих сумм
            for (int i = 0; i < indI; i++) //найдем суммы модулей элементов в строках
            {
                for (int j = 0; j < indI; j++)
                {
                    sumCiStrok[i] += Math.Abs(SysTi[i][j]);
                }
            }
            maxSum = sumCiStrok[0];
            for (int i = 0; i < indI; i++)
            {
                if (maxSum < sumCiStrok[i]) { maxSum = sumCiStrok[i]; }
            }

            //посчитаем норму вектора d
            double maxD; //максимальный из модулей элементов d   
            maxD = Math.Abs(SysTi[0][indJ-1]);
            for (int i = 0; i < indI; i++)
            {
                if (maxD < Math.Abs(SysTi[i][indJ - 1])) { maxD = Math.Abs(SysTi[i][indJ - 1]); }
            }
            //Console.Write(maxSum + " <- "); //Console.Write(maxD + " <-- ");
            if (maxSum == 1) { maxSum = 0.55; } 
            //Вычислим число итераций для точности 0,001
            double NumIterat; //число итераций
            NumIterat = ( Math.Log((1 - maxSum) * (0.001 / maxD)) / Math.Log(maxSum) ) - 1;
            int NumIterat2 = (int)NumIterat + 1; //минимальное число итераций
            //label1.Text = "NI1 " + NumIterat + "NI2 " + NumIterat2 + " q " + maxSum + " ||D|| " + maxD;

            //примем за начальное решение x0 вектор d
            for (int i = 0; i < indI; i++)
            {
                Xi[i] = SysTi[i][indJ - 1];
            }

            //Начнем проход итераций
            int Iterac = 0; //счетчик итераций
            double[] CopyXi = new double[indI];


            while (Iterac < NumIterat2)
            {
                for (int i = 0; i < indI; i++) //скопируем вектор решений, чтобы работать с неизменными значениями
                {
                    CopyXi[i] = Xi[i];
                    Xi[i] = 0;
                }

                for (int i = 0; i < indI; i++) //подставляем в систему значения Xi, принимая результат за новое решение и т.д.
                {
                    for (int j = 0; j < indI; j++)
                    {
                        Xi[i] += SysTi[i][j] * CopyXi[j];
                    }
                    Xi[i] += SysTi[i][indJ - 1]; //добавим элемент d
                }

                Iterac++;
            }

            for (int i = 0; i < indI; i++) //вывод матрицы SysTi вида Cx+d
            {
               Console.Write(Xi[i] + " ");
                Console.WriteLine();
            }

            //Выводим решение метода
            label4.Text = "";
            for (int j = 0; j < indJ - 1; j++)
            {
                label4.Text += " X" + (j+1) + " = " + Math.Round(Xi[j], 3) + " ";
            }
            for (int i = 0; i < indI; i++) //запомнили решение для сравнения
            {
                solutMetod3[i] = Xi[i];
            }

        }

        //-----------
        private void button5_Click(object sender, EventArgs e)// решение методом Зейделя
        {
            //Из матрицы А сделаем SysTi вида C*x + d, C-матрица, d, х - векторы  
            SysTi = new double[indI][];  //объявили строки
            double[] Xi = new double[indI]; //решение СЛУ
            ButtonDown[3] = true;

            for (int i = 0; i < indI; i++)
            {
                SysTi[i] = new double[indJ]; //объявили столбцы
                for (int j = 0; j < indJ; j++)
                {
                    SysTi[i][j] = 0;
                }
            }

            for (int i = 0; i < indI; i++)
            {
                for (int j = 0; j < indJ; j++)
                {
                    SysLinEq[i][j] = Convert.ToDouble(dataGridView1[j, i].Value); //построили матрицу из таблицы
                    //Матрица А
                }
            }
            for (int i = 0; i < indI; i++) //вывод матрицы
            {
                for (int j = 0; j < indJ; j++)
                {
                    Console.Write(SysLinEq[i][j] + " ");
                }
                Console.WriteLine();
            }

            //Проверка на диагональное преобладание
            double SummaElemStrok = 0;
            bool NoDiagonPreoblad = false; //нет диагонального преобладания
            for (int i = 0; i < indI; i++)
            {
                SummaElemStrok = 0;
                for (int j = 0; j < indI; j++)
                {
                    if (i != j) { SummaElemStrok += Math.Abs(SysLinEq[i][j]); }
                }
                if (Math.Abs(SysLinEq[i][i]) < SummaElemStrok) { NoDiagonPreoblad = true; i = indI; }
            }

            if (NoDiagonPreoblad == true) //добавим диагональное преобладание для нашей матрицы
            {
                double[][] MatrA = new double[indI][];  //объявили строки
                double[][] MatrAT = new double[indI][];
                double[] VectB = new double[indI]; //вектор b матрицы SysLiqEq

                for (int i = 0; i < indI; i++)
                {
                    MatrA[i] = new double[indJ]; //объявили столбцы
                    MatrAT[i] = new double[indJ];
                    for (int j = 0; j < indI; j++)
                    {
                        MatrA[i][j] = SysLinEq[i][j];   //матрица А
                        MatrAT[i][j] = SysLinEq[j][i];  //матрица А транспонированная
                    }
                }

                for (int i = 0; i < indI; i++)
                {
                    for (int j = 0; j < indJ; j++)
                    {
                        Console.Write(MatrA[i][j] + " ");
                    }
                    Console.WriteLine();
                }
                for (int i = 0; i < indI; i++)
                {
                    for (int j = 0; j < indJ; j++)
                    {
                        Console.Write(MatrAT[i][j] + " ");
                    }
                    Console.WriteLine();
                }

                for (int i = 0; i < indI; i++)
                {
                    VectB[i] = SysLinEq[i][indJ - 1]; //скопируем вектор b 
                }

                for (int i = 0; i < indI; i++)
                {
                    Console.Write(VectB[i] + " ");
                }
                Console.WriteLine();

                for (int i = 0; i < indI; i++) //перемножим матрицы А и АТ
                {
                    for (int j = 0; j < indJ; j++)
                    {
                        SysLinEq[i][j] = 0;
                    }
                }

                for (int i = 0; i < indI; i++) //перемножим матрицы АТ и А  
                {
                    for (int k = 0; k < indI; k++)
                    {
                        for (int j = 0; j < indI; j++)
                        {
                            SysLinEq[i][k] += MatrAT[i][j] * MatrA[j][k];  // SysLiqEq = AT * A
                        }
                    }

                    for (int j = 0; j < indI; j++)
                    {
                        SysLinEq[i][indJ - 1] += MatrAT[i][j] * VectB[j]; //вектор b = AT * предыд. b
                    }
                }

            }
            for (int i = 0; i < indI; i++)
            {
                for (int j = 0; j < indJ; j++)
                {
                    Console.Write(SysLinEq[i][j] + " ");
                }
                Console.WriteLine();
            }

            //Скопируем A в SysTi
            for (int i = 0; i < indI; i++)
            {
                for (int j = 0; j < indJ; j++)
                {
                    SysTi[i][j] = SysLinEq[i][j];
                    //Потом приведем к виду Cx+d
                }
            }
            //Преобразуем к виду Cx+d
            for (int i = 0; i < indI; i++)
            {
                for (int j = 0; j < indI; j++)
                {
                    //переносим левую часть в правую, затем прибавим к обоим частям xi (i - элемента главной диагонали), с 0 ничего не делаем
                    if (i == j) { /*SysTi[i][j] = SysTi[i][j] * (-1); SysTi[i][j] = SysTi[i][j] + 1; */}
                    else { SysTi[i][j] = SysTi[i][j] * (-1); }
                }
            }
            for (int i = 0; i < indI; i++) //вывод матрицы SysTi вида Cx+d
            {
                for (int j = 0; j < indJ; j++)
                {
                    Console.Write(SysTi[i][j] + " ");
                }
                Console.WriteLine();
            }

            //разделим все элементы строки на диагональные
            double DiagTi = 1;
            for (int i = 0; i < indI; i++) //вывод матрицы SysTi вида Cx+d
            {
                if (SysTi[i][i] != 0)
                {
                    DiagTi = SysTi[i][i]; //запомнили диагональный элемент перед занулением
                }
                else { DiagTi = 1; }
                for (int j = 0; j < indJ; j++)
                {
                    if (i == j)
                    {
                        SysTi[i][j] = 0;
                    }
                    else { SysTi[i][j] = SysTi[i][j] / DiagTi; Console.Write(SysTi[i][j] + " 1#! " + DiagTi + " "); }
                }
                Console.WriteLine();
            }

            for (int i = 0; i < indI; i++) //вывод матрицы SysTi вида Cx+d
            {
                for (int j = 0; j < indJ; j++)
                {
                    Console.Write(SysTi[i][j] + " # ");
                }
                Console.WriteLine();
            }

            //посчитаем норму матрицы С, она должна быть < 1
            double[] sumCiStrok = new double[indI]; //сумма модулей элементов строк матрицы C
            double maxSum; //максимальная из предыдущих сумм
            for (int i = 0; i < indI; i++) //найдем суммы модулей элементов в строках
            {
                for (int j = 0; j < indI; j++)
                {
                    sumCiStrok[i] += Math.Abs(SysTi[i][j]);
                }
            }
            maxSum = sumCiStrok[0];
            for (int i = 0; i < indI; i++)
            {
                if (maxSum < sumCiStrok[i]) { maxSum = sumCiStrok[i]; }
            }

            //посчитаем норму вектора d
            double maxD; //максимальный из модулей элементов d   
            maxD = Math.Abs(SysTi[0][indJ - 1]);
            for (int i = 0; i < indI; i++)
            {
                if (maxD < Math.Abs(SysTi[i][indJ - 1])) { maxD = Math.Abs(SysTi[i][indJ - 1]); }
            }
            //Console.Write(maxSum + " <- "); //Console.Write(maxD + " <-- ");

            //Вычислим число итераций для точности 0,001
            double NumIterat; //число итераций
            NumIterat = Math.Log((1 - maxSum) * 0.001 / maxSum) / Math.Log(maxSum) - 1;
            int NumIterat2 = (int)NumIterat + 1; //минимальное число итераций

            //примем за начальное решение x0 вектор d
            for (int i = 0; i < indI; i++)
            {
                Xi[i] = SysTi[i][indJ - 1];
            }

            //Начнем проход итераций
            int Iterac = 0; //счетчик итераций
            int iterXi = 0; //счетчик для смены старых решений (CopyXi) на новые решения (Xi), по времени нахождения Xi
            int indexXi = 0;
            double[] CopyXi = new double[indI];
            for (int i = 0; i < indI; i++) { CopyXi[i] = 0; }

            //посчитаем норму разности векторов Xi и CopyXi
            double normXi; //максимальный из модулей элементов d   
            normXi = Math.Abs(Xi[0]) - Math.Abs(CopyXi[0]);
            for (int i = 0; i < indI; i++)
            {
                if (normXi < (Math.Abs(Xi[i]) - Math.Abs(CopyXi[i]))) { normXi = Math.Abs(Xi[i]) - Math.Abs(CopyXi[i]); }
            }
            //label1.Text = " normXi " + normXi;
            //while (Iterac < NumIterat2)
            while(normXi > 0.0001)
            {
                
                for (int i = 0; i < indI; i++) //скопируем вектор решений, чтобы работать с неизменными значениями
                {
                    CopyXi[i] = Xi[i];
                    Xi[i] = 0;
                    Console.Write(normXi + " !!! ");
                }
                Console.WriteLine();
                
                for (int i = 0; i < indI; i++) //подставляем в систему значения Xi, принимая результат за новое решение и т.д.
                {
                    iterXi = i;
                    for (int j = 0; j < indI; j++)
                    {
                        //if (iterXi > 0) { Xi[i] += SysTi[i][j] * Xi[j]; iterXi--; } //найденный Xi на предыдущем шаге добавляем
                        //else { Xi[i] += SysTi[i][j] * CopyXi[j]; } //иначе добавляем старые
                        if (iterXi == 0) { Xi[i] += SysTi[i][j] * CopyXi[j]; }
                        else if (iterXi > 0) { Xi[i] += SysTi[i][j] * Xi[j]; iterXi--; }
                    }
                    Xi[i] += SysTi[i][indJ - 1]; //добавим элемент d
                }

                normXi = Math.Abs(Xi[0]) - Math.Abs(CopyXi[0]); //считаем норму разности векторов
                for (int i = 0; i < indI; i++)
                {
                    if (normXi < (Math.Abs(Xi[i]) - Math.Abs(CopyXi[i]))) { normXi = Math.Abs(Xi[i]) - Math.Abs(CopyXi[i]); }
                }

                //Iterac++;
            }

            for (int i = 0; i < indI; i++) //вывод матрицы SysTi вида Cx+d
            {
                Console.Write(Xi[i] + " ");
                Console.WriteLine();
            }

            //Выводим решение метода
            label5.Text = "";
            for (int j = 0; j < indJ - 1; j++)
            {
                label5.Text += " X" + (j+1) + " = " + Math.Round(Xi[j], 3) + " ";
            }
            for (int i = 0; i < indI; i++) //запомнили решение для сравнения
            {
                solutMetod4[i] = Xi[i];
            }

        }

        private void button6_Click(object sender, EventArgs e) //выберем наилучшее решение
        {
            double[] Sol1 = new double[indI]; //результат подставленного решения в СЛУ
            double[] Sol2 = new double[indI];
            double[] Sol3 = new double[indI];
            double[] Sol4 = new double[indI];

            for (int i = 0; i < indI; i++) //считаем результат для 1 метода
            {
                for (int j = 0; j < indI; j++)
                {
                    Sol1[i] += solutMetod1[j] * SysLinEq[i][j]; 
                }
            }

            for (int i = 0; i < indI; i++) //считаем результат для 2 метода
            {
                for (int j = 0; j < indI; j++)
                {
                    Sol2[i] += solutMetod2[j] * SysLinEq[i][j];
                }
            }

            for (int i = 0; i < indI; i++) //считаем результат для 3 метода
            {
                for (int j = 0; j < indI; j++)
                {
                    Sol3[i] += solutMetod3[j] * SysLinEq[i][j];
                }
            }

            for (int i = 0; i < indI; i++) //считаем результат для 4 метода
            {
                for (int j = 0; j < indI; j++)
                {
                    Sol4[i] += solutMetod4[j] * SysLinEq[i][j];
                }
            }

            //Сравним результаты на точность посчитанного решения
            double[] precis1 = new double[indI]; //точность решения
            double[] precis2 = new double[indI];
            double[] precis3 = new double[indI];
            double[] precis4 = new double[indI];
            int M1 = 0, M2 = 0, M3 = 0;
            double maxError1 = 0; //максимальная погрешность метода 1
            double maxError2 = 0;
            double maxError3 = 0;
            double maxError4 = 0;

            for (int i = 0; i < indI; i++) //посчитали точность для 1 метода
            {
                precis1[i] = Math.Abs(Math.Abs(SysLinEq[i][indJ - 1]) - Math.Abs(Sol1[i]));
            }

            for (int i = 0; i < indI; i++) //посчитали точность для 2 метода
            {
                precis2[i] = Math.Abs(Math.Abs(SysLinEq[i][indJ - 1]) - Math.Abs(Sol2[i]));
            }

            for (int i = 0; i < indI; i++) //посчитали точность для 3 метода
            {
                precis3[i] = Math.Abs(Math.Abs(SysLinEq[i][indJ - 1]) - Math.Abs(Sol3[i]));
            }

            for (int i = 0; i < indI; i++) //посчитали точность для 4 метода
            {
                precis4[i] = Math.Abs(Math.Abs(SysLinEq[i][indJ - 1]) - Math.Abs(Sol4[i]));
            }


            for (int i = 0; i < indI; i++) //ищем максимальную погрешность в методе 1
            {
                if (precis1[i] > maxError1) { maxError1 = precis1[i]; }
            }
            for (int i = 0; i < indI; i++) //ищем максимальную погрешность в методе 2
            {
                if (precis2[i] > maxError2) { maxError2 = precis2[i]; }
            }
            for (int i = 0; i < indI; i++) //ищем максимальную погрешность в методе 3
            {
                if (precis3[i] > maxError3) { maxError3 = precis3[i]; }
            }
            for (int i = 0; i < indI; i++) //ищем максимальную погрешность в методе 4
            {
                if (precis4[i] > maxError4) { maxError4 = precis4[i]; }
            }

            double ErM1 = 0, ErM2 = 0; //для запоминания ошибки при сравнениии точностей
            //Сравнение точностей
            if (maxError1 <= maxError2) { M1 = 1; ErM1 = maxError1; } //смотрим, чтобы ошибка была минимальной
            else { M1 = 2; ErM1 = maxError2; } //сравниваем 1 и 2 метод
            
            if (maxError3 <= maxError4) { M2 = 3; ErM2 = maxError3; }
            else { M2 = 4; ErM2 = maxError4; } //сравниваем 3 и 4 метод

            if (ErM1 <= ErM2) { M3 = M1; } //находим лучшее решение
            else { M3 = M2; }

            //Выводим лучшее решение
            label6.Text = "";
            label7.Text = "";
            if (ComprSymmetrMatr == true && MinusUndRoot == false)
            {
                for (int i = 0; i < indI; i++)
                {
                    label7.Text = "Решение методом квадратного корня";
                    label6.Text += " X" + (i + 1) + " = " + Math.Round(solutMetod1[i], 3) + " ";
                }
            }
            else
            {
                if (M3 == 1 && ComprSymmetrMatr == true && MinusUndRoot == false)
                {
                    for (int i = 0; i < indI; i++)
                    {
                        label7.Text = "Решение методом квадратного корня";
                        label6.Text += " X" + (i + 1) + " = " + Math.Round(solutMetod1[i], 3) + " ";
                    }
                }
                if (M3 == 2)
                {
                    for (int i = 0; i < indI; i++)
                    {
                        label7.Text = "Решение методом LU-разложения";
                        label6.Text += " X" + (i + 1) + " = " + Math.Round(solutMetod2[i], 4) + " ";
                    }
                }
                if (M3 == 3)
                {
                    for (int i = 0; i < indI; i++)
                    {
                        label7.Text = "Решение методом простых итераций";
                        label6.Text += " X" + (i + 1) + " = " + Math.Round(solutMetod3[i], 3) + " ";
                    }
                }
                if (M3 == 4)
                {
                    for (int i = 0; i < indI; i++)
                    {
                        label7.Text = "Решение методом Зейделя";
                        label6.Text += " X" + (i + 1) + " = " + Math.Round(solutMetod4[i], 3) + " ";
                    }
                }
            }

        }
    }
}
