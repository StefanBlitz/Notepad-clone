using System.Collections.ObjectModel;
using System.IO;

namespace NotepadClone.Models
{
    public class FileSystemItem
    {
        public string Name { get; }
        public string FullPath { get; }
        public bool IsDirectory { get; }
        public ObservableCollection<FileSystemItem> Children { get; }

        public FileSystemItem(string path)
        {
            FullPath = path;
            Name = Path.GetFileName(path);
            if (string.IsNullOrEmpty(Name))
                Name = path;

            IsDirectory = Directory.Exists(path);
            Children = new ObservableCollection<FileSystemItem>();
        }
    }
}