using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotepadClone.Models;
using NotepadClone.Helpers;
using System.ComponentModel.DataAnnotations;

namespace NotepadClone.ViewModels
{
    public class DocumentViewModel : BaseViewModel
    {
        private string _title;
        private string _content = string.Empty;
        private bool _isModified;
        private string _filePath;



        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayTitle));
            }
        }

        public string Content
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    _content = value;
                    OnPropertyChanged();
                    IsModified = true;
                }
            }
        }

        public bool IsModified
        {
            get => _isModified;
            set
            {
                if (_isModified != value)
                {
                    _isModified = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(DisplayTitle));
                }
            }
        }

        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                OnPropertyChanged();
            }
        }

        public string DisplayTitle => IsModified ? $"{Title}*" : Title;
    }
}
