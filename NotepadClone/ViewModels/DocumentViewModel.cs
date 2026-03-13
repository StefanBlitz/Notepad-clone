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
        #region Variables
        private string _title;
        private string _content = string.Empty;
        private bool _isModified;
        private string _filePath;
        #endregion


        #region Title
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
        #endregion

        #region Content
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
        #endregion

        #region IsModified
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
        #endregion

        #region FilePath
        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Line
        private int _line;
        public int Line
        {
            get => _line;
            set
            {
                _line = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Column
        private int _column;
        public int Column
        {
            get => _column;
            set
            {
                _column = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region CharacterCount
        private int _characterCount;
        public int CharacterCount
        {
            get => _characterCount;
            set
            {
                _characterCount = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Selection
        private int _selectionStart;
        public int SelectionStart
        {
            get => _selectionStart;
            set
            {
                if (_selectionStart != value)
                {
                    _selectionStart = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _selectionLength;
        public int SelectionLength
        {
            get => _selectionLength;
            set
            {
                if (_selectionLength != value)
                {
                    _selectionLength = value;
                    OnPropertyChanged();
                }
            }
        }
#endregion



        public string DisplayTitle => IsModified ? $"{Title}*" : Title;
    }
}
