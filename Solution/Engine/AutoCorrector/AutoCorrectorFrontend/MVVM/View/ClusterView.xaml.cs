using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AutoCorrectorEngine;
using AutoCorrectorFrontend.MVVM.Model;

namespace AutoCorrectorFrontend.MVVM.View
{
    /// <summary>
    /// Interaction logic for ClusterView.xaml
    /// </summary>
    public partial class ClusterView : UserControl
    {
        public ClusterView()
        {
            InitializeComponent();

            this.SizeChanged += MainWindow_SizeChanged;
            this.Loaded += (s, e) => DrawGraph();
        }

        public ObservableCollection<Node> Nodes { get; } = new ObservableCollection<Node>();
        private void DrawGraph()
        {

           List<List<double>> adjacencyMatrix = Settings.AdjacencyMatrixStudSim;

            Canvas.Children.Clear();
            Nodes.Clear();

            int numNodes = adjacencyMatrix.Count;
            double centerX = this.ActualWidth / 4;
            double centerY = this.ActualHeight / 2;
            double radius = Math.Min(centerX, centerY) * 0.8; // 80% of the smallest dimension

            // Calculate the node diameter based on the radius
            double nodeDiameter = radius / 5; // 10% of the radius

            // Add nodes in a circle
            for (int i = 0; i < numNodes; i++)
            {
                double angle = 2 * Math.PI * i / numNodes;
                double x = centerX + radius * Math.Cos(angle) - nodeDiameter / 2;
                double y = centerY + radius * Math.Sin(angle) - nodeDiameter / 2;

                Node node = new Node
                {
                    X = x,
                    Y = y,
                };
                Nodes.Add(node);

                Ellipse ellipse = new Ellipse
                {
                    Width = nodeDiameter,
                    Height = nodeDiameter,
                    Fill = Brushes.Black,
                };
                Canvas.SetLeft(ellipse, x);
                Canvas.SetTop(ellipse, y);
                Canvas.Children.Add(ellipse);

                // Add mouse event handlers
                ellipse.MouseEnter += (sender, e) => { ((Ellipse)sender).Fill = Brushes.Red; };
                ellipse.MouseLeave += (sender, e) => { ((Ellipse)sender).Fill = Brushes.Black; };


                // Create a TextBlock above the node
                TextBlock textBlock = new TextBlock
                {
                    Text = "sanyi",
                    Foreground = Brushes.Red,
                    FontSize = 16
                };
                Canvas.SetLeft(textBlock, x);
                Canvas.SetTop(textBlock, y - 20); // Adjust this value to position the TextBlock above the node
                Canvas.Children.Add(textBlock);

            }

            // Draw edges
            for (int i = 0; i < numNodes; i++)
            {
                for (int j = i + 1; j < numNodes; j++)
                {
                    if (adjacencyMatrix[i][j] == 1)
                    {
                        double nodeRadius = nodeDiameter / 2;
                        Line line = new Line
                        {
                            X1 = Nodes[i].X + nodeRadius,
                            Y1 = Nodes[i].Y + nodeRadius,
                            X2 = Nodes[j].X + nodeRadius,
                            Y2 = Nodes[j].Y + nodeRadius,
                            Stroke = Brushes.Black,
                            StrokeThickness = 6
                        };

                        Canvas.Children.Add(line);

                        // Calculate the midpoint of the line
                        double midX = (Nodes[i].X + Nodes[j].X) / 2;
                        double midY = (Nodes[i].Y + Nodes[j].Y) / 2;

                        // Create a TextBlock at the midpoint
                        TextBlock textBlock = new TextBlock
                        {
                            Text = "kecske",
                            Foreground = Brushes.Red,
                            Visibility = Visibility.Collapsed
                        };
                        Canvas.SetLeft(textBlock, midX);
                        Canvas.SetTop(textBlock, midY - 20); // Adjust this value to position the TextBlock above the edge
                        Canvas.Children.Add(textBlock);

                        // Add mouse event handlers to the line
                        line.MouseEnter += (sender, e) => { textBlock.Visibility = Visibility.Visible; };
                        line.MouseLeave += (sender, e) => { textBlock.Visibility = Visibility.Collapsed; };

                        line.MouseEnter += (sender, e) => { ((Line)sender).Stroke = Brushes.Red; };
                        line.MouseLeave += (sender, e) => { ((Line)sender).Stroke = Brushes.Black; };
                    }
                }
            }

        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Redraw the graph whenever the window size changes
            DrawGraph();
        }
        
    }
}
