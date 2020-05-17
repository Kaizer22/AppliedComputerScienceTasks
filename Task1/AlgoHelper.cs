using System;
using System.Collections.Generic;
using System.Linq;


namespace AppliedComputerScienceTasks
{
    //Класс, объединяющий в себе алгоритмы на графах
    static class AlgoHelper
    {
        private static bool DFS_depth_marker;

        //Метод, осуществляющий поиск в глубину пути до указанного узла
        public static void DFS(Graph graph)
        {
            int[] marker = new int[graph.nodes_num];
            List<int> path = new List<int>();
            int destination;
            DFS_depth_marker = false;

            Console.WriteLine("Введите номер узла назначения:");
            destination = Convert.ToInt32(Console.ReadLine());

            DFSVisit(graph, 0, destination, marker, path);

            Console.WriteLine("Возможно, путь был найден");
            

        }

        //Посещение узла графа
        private static void DFSVisit(Graph graph, int start, int destination, int[] marker, List<int> path)
        {

            if (start == destination && !DFS_depth_marker)
            {
                path.Add(start);
                Console.WriteLine("Путь найден!");
                foreach (int i in path)
                {
                    Console.Write(i);
                }
                Console.WriteLine();
                DFS_depth_marker = true;
            }
            else if (marker[start] != 1 && !DFS_depth_marker)
            {
                marker[start] = 1;
                if (graph.relations.Keys.Contains(start)){
                    foreach (int local_dest in graph.relations[start])
                    {
                        if (local_dest != start)
                        {
                            path.Add(start);
                            DFSVisit(graph, local_dest, destination, marker, path);
                        }
                    }
                }else
                {
                    Console.WriteLine("Внимание! Стартовая вершина отрезана от искомой!");
                }
            }
        }

        //Метод, осуществляющий поиск в ширину на графе и выводящий на экран минимальное кол-во ребер, которое необходимо пересеч,
        //чтобы добраться до n-ого узла
        public static void BFS(Graph graph)
        {
            int[] distance = new int[graph.nodes_num];
            int[] marker = new int[graph.nodes_num];

            Queue<int> not_visited = new Queue<int>();
            not_visited.Enqueue(0);
            distance[0] = 0;
            marker[0] = 1;

            while ( not_visited.Count != 0)
            {
                int v = not_visited.Dequeue();
                
                for (int i = 0; i < graph.relations[v].Count; i++)
                {
                    if (marker[graph.relations[v][i]]== 0){
                        distance[graph.relations[v][i]] = distance[v] + 1;
                        marker[graph.relations[v][i]] = 1;
                        not_visited.Enqueue(graph.relations[v][i]);
                    }  
                }
            }
            int k = 0;
            foreach(int d in distance)
            {
                Console.WriteLine("До вершины {0} - {1}", k, d);
                k++;

            }
        }

        //Метод, реализующий алгоритм Дейкстры на графе
        //На экран выводится кратчайший путь (последовательные номера посещенных вершин) до n-ого узла из указанного стартового узла
        public static void Dijkstra(Graph graph, int start)
        {
            double[] path_cost = new double[graph.nodes_num];
            int[] marker = new int[graph.nodes_num];
            for(int i = 0; i < graph.nodes_num; i++)
                path_cost[i] = int.MaxValue;
            path_cost[start] = 0;

            DijkstraVisit(graph, start, path_cost, marker);

            int k = 0;
            foreach (double w in path_cost)
            {
                Console.WriteLine("Путь до вершины {0} - {1} у.ед.", k, w);
                k++;
            }
        }

        //Метод, осуществляющий рекурсивный обход всех доступных еще непосещенных узлов
        private static void DijkstraVisit(Graph graph, int start,  double[] path_cost, int[] marker)
        {
            int k = 0;
            double[] ways = new double[graph.nodes_num];
            for (int i = 0; i < graph.nodes_num; i++)
            {
                ways[i] = int.MaxValue;
            }

            if (marker[start] == 0)
            {
                foreach (int dest in graph.relations[start])
                {
                    if (path_cost[dest] > (path_cost[start] + graph.rel_weights.GetElement(start, dest)))
                    {
                        path_cost[dest] = path_cost[start] + graph.rel_weights.GetElement(start, dest);
                    }
                    k++;

                }
                marker[start] = 1;
                int min = 0; ;
                for (int i = 0; i < ways.Length; i++)
                {
                    if (marker[i] == 0)
                    {
                        ways[i] = path_cost[i];
                    }
                    else
                    {
                        ways[i] = int.MaxValue;
                    }
                    if (ways[i] < ways[min]) { min = i; }
                }
                DijkstraVisit(graph, min, path_cost, marker);
            }
        }
    }
}
