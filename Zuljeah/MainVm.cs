using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Eos.Mvvm;
using Eos.Mvvm.Attributes;
using Eos.Mvvm.Commands;
using Eos.Mvvm.EventArgs;
using Eos.Mvvm.UiModel;
using Hsp.Reaper.ApiClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Zuljeah;

public class MainVm : ViewModelBase
{

  private ReaperApiClient Client { get; }

  public UiCommandCategory ActionRoot { get; }



  public ObservableCollection<IPage> Pages { get; } = new();

  public IPage? CurrentPage
  {
    get => GetAutoFieldValue<IPage>();
    set
    {
      SetAutoFieldValue(value);
      var cp = CurrentPage;
      if (cp != null)
        Task.Run(async () =>
        {
          await cp.Activate();
          await cp.Refresh();
        });
      UpdateActions();
    }
  }


  public MainVm(ReaperApiClient client)
  {
    Client = client;
    ActionRoot = new UiCommandCategory();

    Task.Run(async () =>
    {
      await client.RegisterCallback("Tick", TimeSpan.FromMilliseconds(125), UpdateTransportInfo);
    });
  }


  public async Task Initialize(string[] args)
  {
    await AddPage(App.Services.GetRequiredService<PlayerPage>());
    var filename = args.FirstOrDefault();
    var bindings = App.Services.GetRequiredService<ActionBindingsEditor>();
    await bindings.Load();
    if (!String.IsNullOrEmpty(filename))
      await LoadSetlist(filename);
  }


  private void UpdateActions()
  {
    ActionRoot.Pages.Clear();

    var container = new CommandContainer();
    if (CurrentPage != null)
      container.CollectFrom(CurrentPage);
    container.CollectFrom(this);

    ActionRoot.Pages.AddRange(container);
  }

  private async Task UpdateTransportInfo()
  {
    BeatPosInfo? bpi = null;
    try
    {
      bpi = await Client.GetBeatPos();
    }
    catch
    {
      bpi = null;
    }

    await Task.WhenAll(Pages.OfType<PlayerPage>().Select(p => p.UpdateTransport(bpi)));
  }

  internal async Task LoadSetlist(string filename)
  {
    var setlist = App.Services.GetRequiredService<Setlist>();
    await setlist.Load(filename);
    await setlist.UpdateFromReaper();
    if (CurrentPage != null)
      await CurrentPage.Refresh();
  }


  [UiCommand(Caption = "Load", Page = "Setlist", Image = "Import")]
  public async Task LoadFromFile()
  {
    var fre = new FileRequestEventArgs
    {
      Type = FileRequestEventArgs.RequestType.OpenFile,
      Title = "Load from File",
      Filter = "Zuljeah Setlist (*.zuljeah)|*.zuljeah"
    };
    if (await UiSettings.DialogService.RequestFile(fre))
      await LoadSetlist(fre.SelectedPath);
  }

  [UiCommand(Caption = "Save", Page = "Setlist", Image = "Save")]
  public async Task SaveToFile()
  {
    var setlist = App.Services.GetRequiredService<Setlist>();
    var fre = new FileRequestEventArgs
    {
      Type = FileRequestEventArgs.RequestType.SaveFile,
      SelectedPath = setlist.Filename,
      Title = "Save from File",
      Filter = "Zuljeah Setlist (*.zuljeah)|*.zuljeah"
    };
    if (await UiSettings.DialogService.RequestFile(fre))
    {
      await setlist.Save(fre.SelectedPath);
      if (CurrentPage != null)
        await CurrentPage.Refresh();
    }
  }

  [UiCommand(Caption = "Edit Setlist", Page = "Setlist", Image = "Change")]
  public async Task ShowSetlistEditorPage()
  {
    await AddPage(new SetlistEditorPage());
  }

  [UiCommand(Caption = "Edit Bindings", Page = "Bindings", Image = "Change")]
  public async Task ShowBindingEditorPage()
  {
    var editor = App.Services.GetRequiredService<ActionBindingsEditor>();
    await AddPage(editor);
  }

  public async Task AddPage(IPage page)
  {
    Pages.Add(page);
    CurrentPage = await ValueTask.FromResult(page);
  }

  public async Task ClosePage(IPage page)
  {
    if (Pages.Contains(page)) Pages.Remove(page);
    CurrentPage = Pages.FirstOrDefault();
    if (CurrentPage != null)
      await CurrentPage.Activate();
  }

}