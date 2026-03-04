using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NotepadClone.Services
{
    public interface IDialogService
    {
        MessageBoxResult Show(string message, string title);
        string ShowInputDialog(string title, string message);
    }
}
