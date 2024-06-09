using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using AutoCorrectorFrontend.Events;
using AutoCorrectorFrontend.MVVM.Converters;
using AutoCorrectorFrontend.MVVM.Model;
using AutoCorrectorFrontend.MVVM.ViewModel;
using Caliburn.Micro;
using Microsoft.Extensions.DependencyInjection;

namespace AutoCorrectorFrontend.MVVM.View
{
    public partial class ClusterView : UserControl
    {
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            var row = (AutoCorrectorEngine.PlagiarismChecker.PlagiarismPair)button.DataContext;

            IEventAggregator eventAggregator = ((App)Application.Current).ServiceProvider.GetRequiredService<EventAggregator>();

            await eventAggregator.PublishAsync(new NavigationRequestEvent(typeof(PlagiarismViewModel), row), marshal: async action => await action());
        }
        public ClusterView()
        {
            InitializeComponent();

            this.SizeChanged += MainWindow_SizeChanged;
            this.Loaded += (s, e) => DrawGraph();
        }

        public ObservableCollection<Node> Nodes { get; } = new ObservableCollection<Node>();
        public ObservableCollection<Edge> Edges { get; } = new ObservableCollection<Edge>();

        List<Line> Lines { get; set; } = new List<Line>();

        public Line CreateLine(Node node1, Node node2, double nodeRadius, SolidColorBrush color, double strokeThickness)
        {
            // Calculate the difference in x and y coordinates between the two nodes
            double dx = node2.X - node1.X;
            double dy = node2.Y - node1.Y;

            // Calculate the distance between the centers of the nodes
            double distance = Math.Sqrt(dx * dx + dy * dy);

            // Calculate the unit vector components in the direction from node1 to node2
            double ux = dx / distance;
            double uy = dy / distance;

            // Calculate the start and end points of the line on the edge of the nodes
            double startX = node1.X + nodeRadius * ux;
            double startY = node1.Y + nodeRadius * uy;
            double endX = node2.X - nodeRadius * ux;
            double endY = node2.Y - nodeRadius * uy;

            // Create and return the line
            return new Line
            {
                X1 = startX + nodeRadius,
                Y1 = startY + nodeRadius,
                X2 = endX + nodeRadius,
                Y2 = endY + nodeRadius,

                Stroke = color,
                StrokeThickness = strokeThickness
            };
        }
        private void DrawGraph()
        {

            var viewModel = (ClusterViewModel)this.DataContext;

            Canvas.Children.Clear();
            Nodes.Clear();

            int numNodes = viewModel.Students.Count;
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
                    Name = viewModel.Students[i].Name,
                };
                Nodes.Add(node);

                Ellipse ellipse = new Ellipse
                {
                    Width = nodeDiameter,
                    Height = nodeDiameter,
                    Fill = Brushes.Black,
                    Stroke = Brushes.White,
                    StrokeThickness = 2,
                };
                Canvas.SetLeft(ellipse, x);
                Canvas.SetTop(ellipse, y);
                Canvas.Children.Add(ellipse);

                // Add mouse event handlers
                ellipse.MouseEnter += (sender, e) => { ((Ellipse)sender).Fill = Brushes.Red; };
                ellipse.MouseLeave += (sender, e) => { ((Ellipse)sender).Fill = Brushes.Black; };

            }

            // Draw edges
            for (int i = 0; i < numNodes; i++)
            {
                for (int j = i + 1; j < numNodes; j++)
                {

                    Edge edge = new Edge
                    {
                        Name1 = Nodes[i].Name,
                        Name2 = Nodes[j].Name,
                    };

                    double nodeRadius = nodeDiameter / 2;

                    var plagPair = viewModel.PlagiarismPairs.First(x => (x.Id1 == edge.Name1 && x.Id2 == edge.Name2) || (x.Id1 == edge.Name2 && x.Id2 == edge.Name1));

                    int avgSim = plagPair.Average_similarity;

                    byte red = (byte)(255 - 2 * avgSim);
                    Color color = Color.FromRgb(red, 0, 0);

                    double strokeThickness = 0.1 + 0.20 * avgSim;


                    var line = CreateLine(Nodes[i], Nodes[j], nodeRadius, new SolidColorBrush(color), strokeThickness);

                    Canvas.Children.Add(line);
                    Lines.Add(line);
                    Edges.Add(edge);

                    // Calculate the midpoint of the line
                    double midX = (Nodes[i].X + Nodes[j].X) / 2;
                    double midY = (Nodes[i].Y + Nodes[j].Y) / 2;

                    // Add mouse event handlers to the line
                    line.MouseEnter += (sender, e) =>
                    {
                        int index = Lines.IndexOf((Line)sender);
                        var edge = Edges[index];
                        var plagPair = viewModel.PlagiarismPairs.First(x => (x.Id1 == edge.Name1 && x.Id2 == edge.Name2) || (x.Id1 == edge.Name2 && x.Id2 == edge.Name1));

                        int rowIndex = viewModel.PlagiarismPairs.IndexOf(plagPair);

                        DataGridRow row = Extension.GetRow(dataGrid, rowIndex);

                        row.Background = Brushes.Red;

                    };

                    //line.MouseEnter += (sender, e) => { ((Line)sender).Stroke = Brushes.Red; };
                    //line.MouseLeave += (sender, e) => { ((Line)sender).Stroke = Brushes.Black; };

                    line.MouseLeave += (sender, e) =>
                    {
                        int index = Lines.IndexOf((Line)sender);
                        var edge = Edges[index];
                        var plagPair = viewModel.PlagiarismPairs.First(x => (x.Id1 == edge.Name1 && x.Id2 == edge.Name2) || (x.Id1 == edge.Name2 && x.Id2 == edge.Name1));

                        int rowIndex = viewModel.PlagiarismPairs.IndexOf(plagPair);

                        DataGridRow row = Extension.GetRow(dataGrid, rowIndex);

                        row.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3F3F46"));
                    };

                }
            }

            for (int i = 0; i < numNodes; i++)
            {

                TextBlock textBlock = new TextBlock
                {
                    Text = Nodes[i].Name,
                    Foreground = Brushes.White,
                    FontSize = 16
                };

                if (Nodes[i].Y < centerY)
                {
                    Canvas.SetTop(textBlock, Nodes[i].Y - 25);
                }
                else
                {
                    Canvas.SetTop(textBlock, Nodes[i].Y + nodeDiameter + 5);
                }

                Canvas.SetLeft(textBlock, Nodes[i].X - Nodes[i].Name.Length);
                Canvas.Children.Add(textBlock);

            }
        }




        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Redraw the graph whenever the window size changes
            DrawGraph();
        }

    }
}
