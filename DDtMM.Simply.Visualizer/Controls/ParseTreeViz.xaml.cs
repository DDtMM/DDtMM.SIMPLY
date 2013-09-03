using DDtMM.SIMPLY.Visualizer.Model;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DDtMM.SIMPLY.Visualizer.Controls
{
    /// <summary>
    /// Interaction logic for ParseTreeViz.xaml
    /// </summary>
    public partial class ParseTreeViz : UserControl
    {
        public ParseTreeViz()
        {
            InitializeComponent();
        }




        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            switch ((string)item.CommandParameter)
            {
                case "Expand":
                    ParserNodeModel node = (ParserNodeModel) item.DataContext;
                    node.ExpandAll();
                    break;
                default:
                    throw new Exception(string.Format("Unknown command {0}.", item.CommandParameter));
                    break;
            }
        }
    }
}
