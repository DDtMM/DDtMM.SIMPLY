using DDtMM.SIMPLY.Rules;
using DDtMM.SIMPLY.Tokens;
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
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.IO;
using DDtMM.SIMPLY.Visualizer.Controls;

namespace DDtMM.SIMPLY.Visualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string codeFile;
        public string parserFile;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ParserNodeReporter.Current.NodeAdded += Current_NodeAdded;
            ParserNodeReporter.Current.NodeRemoved += Current_NodeRemoved;
            ParserModel parserModel = new ParserModel(new Parser());
            DataContext = parserModel;

        }

        void Current_NodeRemoved(object sender, ParserNodeReporter.ParserNodeEventArgs e)
        {
            Console.WriteLine("Removed " + e.Node);
        }

        void Current_NodeAdded(object sender, ParserNodeReporter.ParserNodeEventArgs e)
        {
            Console.WriteLine("Added " + e.Node.StartToken);
        }

        private void BuildParserButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ((ParserModel)DataContext).BuildGrammar();
                TokensTab.IsSelected = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to parse code\n" + ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                GrammarTab.IsSelected = true;
            }
        }

        private void ParseCodeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ((ParserModel)DataContext).ParseCode();
                ParseTreeTab.IsSelected = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to parse code\n" + ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                CodeTab.IsSelected = true;
            }
        }

  
        //private void ParserInterface_FileSave(object sender, FileEventArgs e)
        //{
        //    ((FileInterface)sender).SaveContent(((ParserModel)DataContext).Grammar);
        //}

        //private void ParserInterface_FileOpen(object sender, FileEventArgs e)
        //{
        //    string code = "";
        //    if (((FileInterface)sender).OpenContent(out code))
        //    {
        //        ((ParserModel)DataContext).Grammar = code;
        //    }
        //}

        //private void CodeFile_FileSave(object sender, FileEventArgs e)
        //{
        //    ((FileInterface)sender).SaveContent(((ParserModel)DataContext).Code);
        //}

        //private void CodeFile_FileOpen(object sender, FileEventArgs e)
        //{
        //    string code = "";
        //    if (((FileInterface)sender).OpenContent(out code))
        //    {
        //        ((ParserModel)DataContext).Code = code;
        //    }
        //}
    }
}
