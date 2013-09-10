using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DDtMM.SIMPLY.Visualizer.Controls
{
    public class FileInterface : FrameworkElement
    {
        public enum FileTypes
        {
            Text, Object
        }

        public event EventHandler<FileEventArgs> FileSave;
        public event EventHandler<FileEventArgs> FileSaved;
        public event EventHandler FileSaveCancelled;
        public event EventHandler<FileEventArgs> FileOpen;
        public event EventHandler<FileEventArgs> FileOpened;
        public event EventHandler FileOpenCancelled;

        public static RoutedCommand Open = new RoutedCommand();
        public static RoutedCommand Save = new RoutedCommand();
        public static RoutedCommand SaveAs = new RoutedCommand();

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FileName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(FileInterface), 
            new PropertyMetadata(null, (o, e) => ((FileInterface)o).OnFileNameChanged()));



        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Filter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(string), typeof(FileInterface),
            new PropertyMetadata("All Files|*.*", (o, e) => ((FileInterface)o).OnFilterChanged()));

     
        public object FileData
        {
            get { return GetValue(FileDataProperty); }
            set { SetValue(FileDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FileData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileDataProperty =
            DependencyProperty.Register("FileData", typeof(object), typeof(FileInterface), new PropertyMetadata(null));

        public FileTypes FileType { get; set; }
  
        OpenFileDialog ofd;
        SaveFileDialog sfd;

        public FileInterface()
        {
            FileType = FileTypes.Text;
            CommandBindings.Add(new CommandBinding(Save, SaveAsExecuted, SaveCanExecute));
            CommandBindings.Add(new CommandBinding(SaveAs, SaveAsExecuted));
            CommandBindings.Add(new CommandBinding(Open, OpenExecuted));
            ofd = new OpenFileDialog();
            sfd = new SaveFileDialog();
        }

        public async Task<bool> SaveFileDataAsync()
        {
            byte[] data;
            if (FileType == FileTypes.Text)
            {
                data = System.Text.Encoding.UTF8.GetBytes((string)FileData);
            }
            else
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    new BinaryFormatter().Serialize(ms, FileData);
                    data = ms.ToArray();
                }
            }

            using (FileStream s = File.Open(FileName, FileMode.OpenOrCreate))
            {
                s.Seek(0, SeekOrigin.End);
                await s.WriteAsync(data, 0, data.Length);
  
            }

            return true;
        }

        public async Task<bool>  OpenFileDataAsync()
        {
            byte[] bytes;
            using (FileStream s = File.Open(FileName, FileMode.Open))
            {
                bytes = new byte[(int)s.Length];
                await s.ReadAsync(bytes, 0, (int)s.Length);
            }

            if (FileType == FileTypes.Text)
            {
                FileData = System.Text.Encoding.UTF8.GetString(bytes);
            }
            else
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    FileData = new BinaryFormatter().Deserialize(ms);
                }
            }

            return true;
        }

        private void OnFileNameChanged() 
        {
            ofd.FileName = FileName;
            sfd.FileName = FileName;
        }

        private void OnFilterChanged()
        {
            sfd.Filter = Filter;
            ofd.Filter = Filter;
        }

        #region Open
        private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ShowOpen();
            e.Handled = true;
        }

        private void ShowOpen()
        {
            if (ofd.ShowDialog().Value)
            {
                FileName = ofd.FileName;
                //OpenFileDataAsync().ContinueWith(OnOpenComplete);
                OpenFileDataAsync();
                //OnOpenComplete(
                if (FileOpen != null) FileOpen(this, new FileEventArgs(FileName));
            }
            else
            {
                if (FileOpenCancelled != null) FileOpenCancelled(this, EventArgs.Empty);
            }

        }

        private void OnOpenComplete(Task<bool> openTask)
        {
            if (openTask.IsFaulted)
            {
                MessageBox.Show("Unable to open.\n" + openTask.Exception.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (openTask.Result && FileSaved != null) FileSaved(this, new FileEventArgs(FileName));

        }
        #endregion

        #region SaveAs
        private void SaveAsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ShowSaveAs();
            SaveFileDataAsync().ContinueWith(OnSaveComplete);
            e.Handled = true;
        }

        private void ShowSaveAs()
        {
            if (sfd.ShowDialog().Value)
            {
                FileName = sfd.FileName;
                if (FileSave != null) FileSave(this, new FileEventArgs(FileName));
            }
            else
            {
                if (FileSaveCancelled != null) FileSaveCancelled(this, EventArgs.Empty);
            }

        }
        #endregion

        #region Save
        private void SaveCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !String.IsNullOrWhiteSpace(FileName);
            e.Handled = true;
        }

        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDataAsync().ContinueWith(OnSaveComplete);
            if (FileSave != null) FileSave(this, new FileEventArgs(FileName));
            e.Handled = true;
        }
        #endregion

        private void OnSaveComplete(Task<bool> saveTask)
        {
            if (saveTask.IsFaulted)
            {
                MessageBox.Show("Unable to save.\n" + saveTask.Exception.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (saveTask.Result && FileSaved != null) FileSaved(this, new FileEventArgs(FileName));
            
        }

    }

    public class FileEventArgs : EventArgs
    {
        public string FileName;

        public FileEventArgs(string fileName)
        {
            FileName = fileName;
        }
    }
}
