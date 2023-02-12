using System;
using System.Threading.Tasks;
using Eos.Mvvm.Dialogs;
using Eos.Mvvm.EventArgs;
using Ookii.Dialogs.Wpf;

namespace Zuljeah;

internal class BasicDialogService : DialogService
{
  public override async Task<bool> RequestFile(FileRequestEventArgs fre)
  {
    string defaultExt = "";
    if (!String.IsNullOrEmpty(fre.Filter))
    {
      var parts = fre.Filter.Split('|');
      defaultExt = parts[1].Substring(1);
    }

    if (fre.Type == FileRequestEventArgs.RequestType.OpenFile)
    {
      var dlg = new VistaOpenFileDialog
      {
        Title = fre.Title,
        FileName = fre.SelectedPath,
        Filter = fre.Filter
      };
      if (dlg.ShowDialog() == true)
      {
        fre.SelectedPath = dlg.FileName;
        return true;
      }
    }

    if (fre.Type == FileRequestEventArgs.RequestType.SaveFile)
    {
      var dlg = new VistaSaveFileDialog
      {
        Title = fre.Title,
        FileName = fre.SelectedPath,
        Filter = fre.Filter,
        DefaultExt = defaultExt
      };
      if (dlg.ShowDialog() == true)
      {
        fre.SelectedPath = dlg.FileName;
        return true;
      }
    }

    return false;
  }

  public override Task HandleException(Exception ex)
  {
    throw new NotImplementedException();
  }

}