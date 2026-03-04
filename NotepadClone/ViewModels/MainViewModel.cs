using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using NotepadClone.Helpers;
using System.Windows.Input;
using NotepadClone.Commands;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using NotepadClone.Services;
using System.IO;
using NotepadClone.Models;


namespace NotepadClone.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private DocumentViewModel _selectedDocument;
        private int _fileCounter = 1;
        private string _copiedFolderPath;

        public ObservableCollection<DocumentViewModel> Documents { get; set; } = new ObservableCollection<DocumentViewModel>();
        public ObservableCollection<FileSystemItem> RootItems { get; } = new ObservableCollection<FileSystemItem>();

        public DocumentViewModel SelectedDocument
        {
            get => _selectedDocument;
            set
            {
                if (_selectedDocument != value)
                {
                    _selectedDocument = value;
                    OnPropertyChanged();
                }
            }
        }

        #region Commands

        public ICommand NewFileCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SaveAsCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand CloseAllCommand { get; }
        public ICommand CreateFileInFolderCommand { get; }
        public ICommand CopyPathCommand { get; }
        public ICommand CopyFolderCommand { get; }
        public ICommand PasteFolderCommand { get; }
        public ICommand CreateFolderInFolderCommand { get; }
        public ICommand DeleteItemCommand { get; }

        #endregion

        public MainViewModel()
        {

            _dialogService = new DialogService();

            NewFileCommand = new RelayCommand(_ => CreateNewDocument());
            SaveCommand = new RelayCommand(_ => SaveFile(), _ => SelectedDocument != null);
            SaveAsCommand = new RelayCommand(_ => SaveFileAs(), _ => SelectedDocument != null);
            OpenCommand = new RelayCommand(_ => OpenFile());
            CloseCommand = new RelayCommand(doc =>
            {
                if (doc is DocumentViewModel document)
                    CloseDocument(document);
                else
                    CloseDocument(SelectedDocument);
            });
            CloseAllCommand = new RelayCommand(_ => CloseAllDocuments());

            CreateFileInFolderCommand = new RelayCommand(item => CreateFileInFolder(item as FileSystemItem));
            CopyPathCommand = new RelayCommand(item => CopyPath(item as FileSystemItem));
            CopyFolderCommand = new RelayCommand(item => CopyFolder(item as FileSystemItem));
            PasteFolderCommand = new RelayCommand(item => PasteFolder(item as FileSystemItem));
            CreateFolderInFolderCommand = new RelayCommand(item => CreateFolderInFolder(item as FileSystemItem));
            DeleteItemCommand = new RelayCommand(item => DeleteItem(item as FileSystemItem));

            CreateNewDocument();
            LoadDrives();

        }

        #region File Operations

        private void CreateNewDocument()
        {
            var doc = new DocumentViewModel();
            doc.Title = $"File {_fileCounter++}";
            doc.IsModified = false;

            Documents.Add(doc);
            SelectedDocument = doc;
        }

        private void SaveFile()
        {

            if (SelectedDocument == null)
                return;

            if (string.IsNullOrEmpty(SelectedDocument.FilePath))
            {
                SaveFileAs();
                return;
            }

            File.WriteAllText(SelectedDocument.FilePath, SelectedDocument.Content);
            SelectedDocument.IsModified = false;

        }

        private void SaveFileAs()
        {
            if (SelectedDocument == null)
                return;

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, SelectedDocument.Content);
                SelectedDocument.FilePath = dialog.FileName;
                SelectedDocument.Title = Path.GetFileName(dialog.FileName);
                SelectedDocument.IsModified = false;
            }
        }

        private void OpenFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                string content = File.ReadAllText(dialog.FileName);
                var doc = new DocumentViewModel
                {
                    Title = Path.GetFileName(dialog.FileName),
                    FilePath = dialog.FileName,
                    IsModified = false
                };

                doc.Content = content;
                doc.IsModified = false;

                Documents.Add(doc);
                SelectedDocument = doc;
            }
        }

        private void CloseDocument(DocumentViewModel document)
        {
            if (document == null)
                return;

            if (document.IsModified)
            {
                var result = _dialogService.Show($"Do you want to save changes to {document.Title}?", "Unsaved changes");


                if (result == System.Windows.MessageBoxResult.Cancel)
                    return;

                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    SelectedDocument = document;
                    SaveFile();
                }
            }

            Documents.Remove(document);

            if (Documents.Count == 0)
            {
                _fileCounter = 1;
            }
        }

        private void CloseAllDocuments()
        {
            foreach (var doc in Documents.ToList())
            {
                CloseDocument(doc);

                if (Documents.Contains(doc))
                    return;

            }

        }

        private void LoadDrives()
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    var item = new FileSystemItem(drive.RootDirectory.FullName);
                    item.Children.Add(null);
                    RootItems.Add(item);
                }
            }
        }

        public void LoadDirectory(FileSystemItem item)
        {
            if (!item.IsDirectory)
                return;

            if (item.Children.Count == 1 && item.Children[0] == null)
            {
                item.Children.Clear();

                try
                {
                    foreach (var dir in Directory.GetDirectories(item.FullPath))
                    {
                        var child = new FileSystemItem(dir);
                        child.Children.Add(null);
                        item.Children.Add(child);
                    }

                    foreach (var file in Directory.GetFiles(item.FullPath))
                    {
                        item.Children.Add(new FileSystemItem(file));
                    }
                }
                catch { }
            }
        }

        public void OpenFileFromExplorer(string path)
        {
            if (!File.Exists(path))
                return;

            string content = File.ReadAllText(path);

            var doc = new DocumentViewModel
            {
                Title = Path.GetFileName(path),
                FilePath = path,
                IsModified = false
            };

            doc.Content = content;
            doc.IsModified = false;

            Documents.Add(doc);
            SelectedDocument = doc;
        }

        private void CreateFileInFolder(FileSystemItem item)
        {
            if (item == null || !item.IsDirectory)
                return;

            string fileName = _dialogService.ShowInputDialog("New File", "Enter file name:");

            if (string.IsNullOrWhiteSpace(fileName))
                return;

            if (!fileName.Contains("."))
                fileName += ".txt";

            string newFilePath = Path.Combine(item.FullPath, fileName);

            if (File.Exists(newFilePath))
                return;

            File.WriteAllText(newFilePath, string.Empty);

            item.Children.Add(new FileSystemItem(newFilePath));
        }

        private void CopyPath(FileSystemItem item)
        {
            if (item == null)
                return;

            System.Windows.Clipboard.SetText(item.FullPath);
        }

        private void CopyFolder(FileSystemItem item)
        {
            if (item == null || !item.IsDirectory)
                return;

            _copiedFolderPath = item.FullPath;
        }

        private void PasteFolder(FileSystemItem target)
        {
            if (target == null || !target.IsDirectory)
                return;

            if (string.IsNullOrEmpty(_copiedFolderPath))
                return;

            string sourcePath = Path.GetFullPath(_copiedFolderPath);
            string targetPath = Path.GetFullPath(target.FullPath);

            if (targetPath.StartsWith(sourcePath, StringComparison.OrdinalIgnoreCase))
                return;

            string folderName = Path.GetFileName(sourcePath);
            string destinationPath = Path.Combine(targetPath, folderName);

            int counter = 1;

            while (Directory.Exists(destinationPath))
            {
                destinationPath = Path.Combine(targetPath, $"{folderName}_Copy{counter++}");
            }

            CopyDirectory(sourcePath, destinationPath);

            var newFolder = new FileSystemItem(destinationPath);
            newFolder.Children.Add(null);
            target.Children.Add(newFolder);
        }

        private void CopyDirectory(string source, string destination)
        {
            Directory.CreateDirectory(destination);

            foreach (var file in Directory.GetFiles(source))
            {
                string destFile = Path.Combine(destination, Path.GetFileName(file));
                File.Copy(file, destFile);
            }

            foreach (var dir in Directory.GetDirectories(source))
            {
                string destDir = Path.Combine(destination, Path.GetFileName(dir));
                CopyDirectory(dir, destDir);
            }
        }

        private void CreateFolderInFolder(FileSystemItem item)
        {
            if (item == null || !item.IsDirectory)
                return;

            string folderName = _dialogService.ShowInputDialog("New Folder", "Enter folder name:");

            if (string.IsNullOrWhiteSpace(folderName))
                return;

            string newFolderPath = Path.Combine(item.FullPath, folderName);

            if (Directory.Exists(newFolderPath))
                return;

            Directory.CreateDirectory(newFolderPath);

            var newFolder = new FileSystemItem(newFolderPath);
            newFolder.Children.Add(null);

            item.Children.Add(newFolder);
        }

        private void DeleteItem(FileSystemItem item)
        {
            if (item == null)
                return;

            var result = _dialogService.Show($"Delete {item.Name} ?", "Confirm delete");

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                if (item.IsDirectory)
                    Directory.Delete(item.FullPath, true);
                else
                    File.Delete(item.FullPath);
            }
            catch
            {
                return;
            }

            RefreshParent(item);
        }

        private void RefreshParent(FileSystemItem item)
        {
            var parent = FindParent(RootItems, item);

            if (parent != null)
            {
                parent.Children.Remove(item);
            }
        }

        private FileSystemItem FindParent(ObservableCollection<FileSystemItem> items, FileSystemItem child)
        {
            foreach (var item in items)
            {
                if (item.Children.Contains(child))
                    return item;

                var result = FindParent(item.Children, child);
                if (result != null)
                    return result;
            }
            return null;
        }

        #endregion
    }
}
