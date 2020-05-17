using MathNet.Symbolics;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;
namespace AppliedComputerScienceTasks
{
    public partial class ODEGraphics : Form
    {
        private Color[] ALL_COLORS = new Color[8] {Color.Red, Color.Blue, Color.Black, Color.Aquamarine, 
            Color.Brown, Color.Firebrick, Color.LavenderBlush, Color.Honeydew };

        public ODEGraphics(List<Dictionary<string, FloatingPoint>> approximaPoints, 
            List<Dictionary<string, FloatingPoint>> solutionPoints = null, 
            string TAG_1 = "Приближение решения для {0}",
            string TAG_2 = "Точное решения для {0}")
        {
            InitializeComponent();
            ZedGraphControl zedGraph = new ZedGraphControl();

            zedGraph.Location = new Point(0, 0);
            zedGraph.Name = "zedGraph";
            zedGraph.Size = new Size(600, 600);
            Controls.Add(zedGraph);
            GraphPane myPane = zedGraph.GraphPane;
            myPane.CurveList.Clear();
            myPane.Title.Text = "Графики функций решений \n" +
                "системы ОДУ";
            myPane.XAxis.Title.Text = "X";
            myPane.YAxis.Title.Text = "Y";

            List<List<double>> bufferPoints = new List<List<double>>();
            List<double> single2DPoint = new List<double>();
            int current_color = 0;
            foreach (string key in approximaPoints[0].Keys) {
                if (!key.Equals("x"))
                {
                    foreach (Dictionary<string, FloatingPoint> namedVector in approximaPoints)
                    {
                        single2DPoint.Add(namedVector["x"].RealValue);
                        single2DPoint.Add(namedVector[key].RealValue);
                        bufferPoints.Add(new List<double>(single2DPoint));
                        single2DPoint.Clear();

                    }
                    CreateGraph(myPane, bufferPoints, ALL_COLORS[current_color],
                        string.Format(TAG_1, key));
                    current_color++;
                    bufferPoints.Clear();
                }
            }
            
            if (!(solutionPoints is null))
                foreach (string key in solutionPoints[0].Keys)
                    if (!key.Equals("x"))
                    {
                        foreach (Dictionary<string, FloatingPoint> namedVector in solutionPoints)
                        {
                            single2DPoint.Add(namedVector["x"].RealValue);
                            single2DPoint.Add(namedVector[key].RealValue);
                            bufferPoints.Add(new List<double>(single2DPoint));
                            single2DPoint.Clear();

                        }
                        CreateGraph(myPane, bufferPoints, ALL_COLORS[current_color],
                            string.Format(TAG_2, key));
                        current_color++;
                        bufferPoints.Clear();
                    }
            zedGraph.AxisChange();
            zedGraph.Invalidate();
        }

        private static void CreateGraph(GraphPane myPane, List<List<double>> approximaPoints, Color color, string title)
        {
            double x, y;
            PointPairList list = new PointPairList();
            for (int i = 0; i < approximaPoints.Count; i++)
            {
                x = approximaPoints[i][0];
                y = approximaPoints[i][1];
                list.Add(x, y);
            }

            LineItem myCurve = myPane.AddCurve(title,
               new PointPairList(list), color, SymbolType.Circle);

            myPane.YAxis.Cross = 0.0;
            myPane.Chart.Border.IsVisible = false;
            myPane.XAxis.MajorTic.IsOpposite = false;
            myPane.XAxis.MinorTic.IsOpposite = false;
            myPane.YAxis.MajorTic.IsOpposite = false;
            myPane.YAxis.MinorTic.IsOpposite = false;
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            ClientSize = new Size(578, 544);
            Location = new Point(700, 700);
            Name = "График функций решений (найденное методом Рунге-Кутты 4-го порядка и точное) системы ОДУ";
            ResumeLayout(false);
        }
    }
}
