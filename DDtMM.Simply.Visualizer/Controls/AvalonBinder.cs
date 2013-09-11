using DDtMM.SIMPLY.Visualizer.Model;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace DDtMM.SIMPLY.Visualizer.Controls
{
    public class AvalonBinder : FrameworkElement, IDisposable
    {
        #region Dependency Props
        public static AvalonBinder GetBinder(DependencyObject obj)
        {
            return (AvalonBinder)obj.GetValue(BinderProperty);
        }

        public static void SetBinder(DependencyObject obj, AvalonBinder value)
        {
            obj.SetValue(BinderProperty, value);
        }
        // Using a DependencyProperty as the backing store for Binder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BinderProperty =
            DependencyProperty.RegisterAttached("Binder", typeof(AvalonBinder), typeof(AvalonBinder),
            new PropertyMetadata(null, Binder_Changed));

        public static void Binder_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender.GetType() != typeof(TextEditor)) throw new Exception("Attaches to TextEditor only");

            if (args.OldValue != null)
            {
                ((AvalonBinder)args.OldValue).Dispose();
            }
            if (args.NewValue != null)
            {
                ((AvalonBinder)args.NewValue).Editor = (TextEditor)sender;
            }
        }
        
        public TextEditor Editor
        {
            get { return (TextEditor)GetValue(EditorProperty); }
            set { SetValue(EditorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Editor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditorProperty =
            DependencyProperty.Register("Editor", typeof(TextEditor), typeof(AvalonBinder), 
            new PropertyMetadata(null, EditorProperty_Changed));

        public static void EditorProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args) 
        {
            ((AvalonBinder)sender).SetEditor((TextEditor)args.OldValue, (TextEditor)args.NewValue);
         }

        public string Text
        {
            get 
            {
                TextEditor editor = Editor;
                return (editor != null) ? editor.Text : (string) GetValue(TextProperty);
            }
            set
            {
                TextEditor editor = Editor;
                if (editor != null)
                {
                    if (!editorIgnoreTextChange)
                    {
                        editor.TextChanged -= Editor_TextChanged;
                        editor.Text = value;
                        editor.TextChanged += Editor_TextChanged;
                    }
                    else
                    {
                        // we want subsequent changes
                        editorIgnoreTextChange = false;
                    }
                }
                else SetValue(TextProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(AvalonBinder),
            new PropertyMetadata(null, TextProperty_Changed));

        public static void TextProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((AvalonBinder)sender).Text = (string) args.NewValue;
        }

        public string HighlightingDefinition
        {
            set
            {
                using (StringReader sr = new StringReader(value))
                {
                    using (XmlTextReader xtr = new XmlTextReader(sr))
                    {
                        TextDocument x = new TextDocument();
                        Editor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                            HighlightingLoader.Load(xtr, HighlightingManager.Instance);
                    }
                }

            }  
        }
        #endregion

        /// <summary>
        /// Set to true after Editor updates its property.
        /// </summary>
        private bool editorIgnoreTextChange;

        public AvalonBinder()
        {
            editorIgnoreTextChange = false;
        }
 
        private void SetEditor(TextEditor oldeditor, TextEditor neweditor)
        {
            if (oldeditor != null)
            {
                oldeditor.TextChanged -= Editor_TextChanged;
                oldeditor.DataContextChanged -= Editor_DataContextChanged;
                DataContext = null;
                Text = oldeditor.Text;
            }

            if (neweditor != null)
            {
                neweditor.Text = Text;
                // we only want one copy of text at a time
                SetValue(TextProperty, null);
                neweditor.TextChanged += Editor_TextChanged;
                neweditor.DataContextChanged += Editor_DataContextChanged;
                DataContext = neweditor.DataContext;  
            }
        }

      
        private void Editor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DataContext = e.NewValue;
        }
        private void Editor_TextChanged(object sender, EventArgs e)
        {
            editorIgnoreTextChange = true;
            SetValue(TextProperty, Editor.Text);
            
        }

        public void Dispose()
        {
            Editor = null;
            DataContext = null;
        }
    }
}
