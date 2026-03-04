using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NotepadClone.ViewModels;
using NotepadClone.Models;
using System.Windows.Media;

namespace NotepadClone
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
        }

        private void TabItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                var tabItem = sender as TabItem;
                if (tabItem?.DataContext is DocumentViewModel doc)
                {
                    if (DataContext is MainViewModel vm)
                    {
                        vm.CloseCommand.Execute(doc);
                    }
                }
            }
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is TreeViewItem item &&
                item.DataContext is FileSystemItem fsItem &&
                DataContext is MainViewModel vm)
            {
                vm.LoadDirectory(fsItem);
            }
        }

        private void TreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (FolderTree.SelectedItem is FileSystemItem item &&
                !item.IsDirectory &&
                DataContext is MainViewModel vm)
            {
                vm.OpenFileFromExplorer(item.FullPath);
            }
        }

        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = e.OriginalSource as DependencyObject;

            while (element != null && !(element is TreeViewItem))
                element = VisualTreeHelper.GetParent(element);

            if (element is TreeViewItem item)
            {
                item.IsSelected = true;
                item.Focus();
                e.Handled = true;

                ShowContextMenu(item);
            }
        }

        private void ShowContextMenu(TreeViewItem item)
        {
            var menu = new ContextMenu();

            var newFile = new MenuItem
            {
                Header = "New file",
                InputGestureText = "Ctrl+Shift+N"
            };
            newFile.Click += (s, e) =>
            {
                if (DataContext is MainViewModel vm &&
                    item.DataContext is FileSystemItem fsItem)
                    vm.CreateFileInFolderCommand.Execute(fsItem);
            };

            var newFolder = new MenuItem
            {
                Header = "New folder",
                InputGestureText = "Ctrl+Alt+N"
            };
            newFolder.Click += (s, e) =>
            {
                if (DataContext is MainViewModel vm &&
                    item.DataContext is FileSystemItem fsItem)
                    vm.CreateFolderInFolderCommand.Execute(fsItem);
            };

            var copyPath = new MenuItem
            {
                Header = "Copy path",
                InputGestureText = "Ctrl+Shift+C"
            };
            copyPath.Click += (s, e) =>
            {
                if (DataContext is MainViewModel vm &&
                    item.DataContext is FileSystemItem fsItem)
                    vm.CopyPathCommand.Execute(fsItem);
            };

            var copyFolder = new MenuItem
            {
                Header = "Copy folder",
                InputGestureText = "Ctrl+Shift+F"
            };
            copyFolder.Click += (s, e) =>
            {
                if (DataContext is MainViewModel vm &&
                    item.DataContext is FileSystemItem fsItem)
                    vm.CopyFolderCommand.Execute(fsItem);
            };

            var pasteFolder = new MenuItem
            {
                Header = "Paste folder",
                InputGestureText = "Ctrl+Shift+V"
            };
            pasteFolder.Click += (s, e) =>
            {
                if (DataContext is MainViewModel vm &&
                    item.DataContext is FileSystemItem fsItem)
                    vm.PasteFolderCommand.Execute(fsItem);
            };

            var delete = new MenuItem
            {
                Header = "Delete",
                InputGestureText = "Delete"
            };
            delete.Click += (s, e) =>
            {
                if (DataContext is MainViewModel vm &&
                    item.DataContext is FileSystemItem fsItem)
                    vm.DeleteItemCommand.Execute(fsItem);
            };

            menu.Items.Add(newFile);
            menu.Items.Add(newFolder);
            menu.Items.Add(new Separator());
            menu.Items.Add(copyPath);
            menu.Items.Add(copyFolder);
            menu.Items.Add(pasteFolder);
            menu.Items.Add(new Separator());
            menu.Items.Add(delete);

            item.ContextMenu = menu;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                if (!vm.CanCloseApplication())
                {
                    e.Cancel = true;
                    return;
                }
            }

            base.OnClosing(e);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is TabControl tab &&
                tab.SelectedContent is TextBox tb)
            {
                tb.Focus();
            }
        }

        private void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb &&
                tb.DataContext is DocumentViewModel doc)
            {
                doc.CharacterCount = tb.Text.Length;
            }
        }

        private void Editor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb &&
                tb.DataContext is DocumentViewModel doc)
            {
                int caret = tb.CaretIndex;
                int line = tb.GetLineIndexFromCharacterIndex(caret);
                int column = caret - tb.GetCharacterIndexFromLineIndex(line);

                doc.Line = line + 1;
                doc.Column = column + 1;
            }
        }
    }
}