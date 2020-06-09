using System;
using System.Collections.Generic;

namespace AppliedComputerScienceTasks
{
    class Graph
    {
        //Статичная переменная для присвоения каждому графу уникального id
        private static int counter = 0;

        private int id;
        //Матрицы смежности и инцидентности
        private NumericMatrix adj_table;
        private NumericMatrix inc_table;

        //Кол-во узлов графа
        public int nodes_num { get; }

        private int rel_num { get; set; }
       

        //Матрица весов
        public NumericMatrix rel_weights { get; }

        //Словарь, описывающий связи для каждого узла графа
        public Dictionary<int, List<int>> relations { get; }


        //Конструктор класса, необходимый для инициализации графа через матрицу смежности
        public Graph(int nodes_num)
        {
            this.nodes_num = nodes_num;
            adj_table = new NumericMatrix(nodes_num);
            relations = new Dictionary<int, List<int>>();
            rel_weights = new NumericMatrix(nodes_num);
            id = counter;
            counter++;
        }

        //Конструктор класса, необходимый для инициализации графа через матрицу инцидентности
        public Graph(int nodes_num, int rel_num)
        {
            this.nodes_num = nodes_num;
            this.rel_num = rel_num;
            inc_table = new NumericMatrix(nodes_num, rel_num);
            relations = new Dictionary<int, List<int>>();
            rel_weights = new NumericMatrix(nodes_num);
            id = counter;
            counter++;

        }

        //Иницализация графа с клавиатуры посредством матрицы смежности
        public void InitGraphAdj()
        {
            Console.WriteLine("Заполните матрицу смежности:");
            adj_table = new NumericMatrix(nodes_num);
            adj_table.ConsoleInput();
            GetIncTable();
            CreateRelations();
        }

        //Получение матрицы инцидентности по матрице смежности
        //Обеспечивает более полное описание графа и дает больше гибкости при работе с данным классом
        private void GetIncTable()
        {
            rel_num = 0;
            for (int i = 0; i < nodes_num; i++)
            {
                for (int j = 0; j < nodes_num; j++)
                {
                    if (adj_table.GetElement(i, j) == 1) {
                        rel_num++;
                    }
                }
            }

            inc_table = new NumericMatrix(nodes_num, rel_num);
            int rel_counter = 0;
            for (int i = 0; i < nodes_num; i++)
            {
                for (int j = 0; j < nodes_num; j++)
                {
                    if (adj_table.GetElement(i, j) == 1)
                    {
                        if (i == j)
                        {
                            inc_table.SetElement(i, rel_counter, 2);
                            rel_counter++;
                        } else
                        {
                            inc_table.SetElement(i, rel_counter, -1);
                            inc_table.SetElement(j, rel_counter, 1);
                            rel_counter++;
                        }
                    }
                    if (rel_counter > rel_num)
                    {
                        Console.WriteLine("Warning");
                    }
                }
            }
        }

        //Иницализация графа с клавиатуры посредством матрицы инцидентности
        public void InitGraphInc()
        {
            Console.WriteLine("Введите количество отношений между узлами:");
            rel_num = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Заполните матрицу инцидентности:");
            inc_table = new NumericMatrix(nodes_num, rel_num);
            inc_table.ConsoleInput();
            GetAdjTable();
            CreateRelations();
        }

        //Получение матрицы смежности по матрице инцидентности
        //Обеспечивает более полное описание графа и дает больше гибкости при работе с данным классом
        private void GetAdjTable()
        {
            adj_table = new NumericMatrix(nodes_num);
            int from_node = 0;
            int to_node = 0;
            Console.WriteLine(rel_num);

            for (int j = 0; j < rel_num; j++)
            {
                for (int i = 0; i < nodes_num; i++)
                {
                    if (inc_table.GetElement(i, j) == -1)
                    {
                        from_node = i;
                    } else if (inc_table.GetElement(i, j) == 1)
                    {
                        to_node = i;
                    } else if (inc_table.GetElement(i, j) == 2)
                    {
                        from_node = i;
                        to_node = i;
                    }
                }
                adj_table.SetElement(from_node, to_node, 1);
            }

        }

        //Инициализация весов ребер графа
        public void InitWeights()
        {
            
            Console.WriteLine("Введите значения весов для отношений между узлами:");
            int from_node = 0;
            int to_node = 0;
            double weight;

            for (int j = 0; j < nodes_num; j++)
            {
                for (int i = 0; i < nodes_num; i++)
                {

                    if (adj_table.GetElement(i, j) == 1)
                    {
                        if (i == j)
                        {
                            from_node = i;
                            to_node = i;
                        }
                        else
                        {
                            from_node = i;
                            to_node = j;
                        }
                        Console.WriteLine("Введите вес для отношения между узлами [{0}] и [{1}]:",
                        from_node, to_node);
                        weight = Convert.ToDouble(Console.ReadLine());
                        rel_weights.SetElement(from_node, to_node, weight);
                    }
                }
            }
        }

        //Получение словаря, описывающего ребра графа, по матрице смежности
        //Обеспечивает более полное описание графа и дает больше гибкости при работе с данным классом
        private void CreateRelations()
        {
            for (int i = 0; i < nodes_num; i++)
            {
                for (int j = 0; j < nodes_num; j++)
                {
                    if (adj_table.GetElement(i, j) == 1)
                    {
                        if (relations.ContainsKey(i))
                        {
                            relations[i].Add(j);
                        }
                        else
                        {
                            relations.Add(i, new List<int>());
                            relations[i].Add(j);
                        }
                    }
                }
            }
        }

        public void AddRelation(int from_node, int to_node)
        {

        }

        public void DeleteRelation(int from_node, int to_node)
        {

        }


        public override string ToString()
        {
            string result = "Graph" + id + "\n" + adj_table.ToString() + "\n" + inc_table.ToString() + "\n";
            foreach (int loc in relations.Keys)
            {
                Console.WriteLine("Из вершины {0} в : ", loc);
                foreach (int dest in relations[loc])
                {
                    Console.Write(dest + " ");
                }
                Console.WriteLine();
            }
            return result ;
        }
    }
}
