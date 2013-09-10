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

        private void LoadParserButton_Click(object sender, RoutedEventArgs e)
        {
            string content;
 
            if (ShowAndGetContent(out content, ref parserFile))
            {
                ((ParserModel)DataContext).Grammar = content;
                GrammarTab.IsSelected = true;
            }
        }

        private void LoadCodeButton_Click(object sender, RoutedEventArgs e)
        {
            string content;
   
            if (ShowAndGetContent(out content, ref codeFile))
            {
 
                ((ParserModel)DataContext).Code = content;
            }
        }

        private bool ShowAndGetContent(out string content, ref string file)
        {
            content = null;
            OpenFileDialog ofd = new OpenFileDialog();

            if (file != null) ofd.FileName = file;

            if (ofd.ShowDialog().Value)
            {
                using (Stream s = ofd.OpenFile())
                {
                    using (StreamReader sr = new StreamReader(s))
                    {
                        content = sr.ReadToEnd();
                    }
                }
                file = ofd.FileName;
                return true;
            }
            return false;
        }

        private void SaveParserButton_Click(object sender, RoutedEventArgs e)
        {
            SaveContent(GrammarEditor.Text, ref parserFile);
        }

        private void SaveCodeButton_Click(object sender, RoutedEventArgs e)
        {
            SaveContent(CodeEditor.Text, ref codeFile);
        }

        private bool SaveContent(string content, ref string file)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                if (file != null) sfd.FileName = file;

                if (sfd.ShowDialog().Value)
                {
                    using (var fs = File.OpenWrite(sfd.FileName))
                    {
                        using (var sw = new StreamWriter(fs))
                        {
                            sw.Write(content);
                            file = sfd.FileName;
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to save/ \n" + ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }
    }
}
