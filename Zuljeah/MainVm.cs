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
using Microsoft.Extensions.Options;

namespace Zuljeah;

public class MainVm : ViewModelBase, IHost
{

  public ReaperApiClient Client { get; }

  public Setlist CurrentSetlist { get; } = new();

  public UiCommandCategory ActionRoot { get; }



  public ObservableCollection<IPage> Pages { get; } = new();

  private ZuljeahConfiguration Configuration { get; }

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


  public MainVm(IOptions<ZuljeahConfiguration> config)
  {
    ActionRoot = new UiCommandCategory();
    Configuration = config.Value;
    Client = new ReaperApiClient(config.Value.ReaperUri);

    Task.Run(async () =>
    {
      await Client.RegisterCallback("Tick", TimeSpan.FromMilliseconds(125), UpdateTransportInfo);
    });
  }


  public async Task Initialize(string[] args)
  {
    await AddPage(new PlayerPage(this, Configuration));
    var filename = args.FirstOrDefault();
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

    //var cp = CurrentPage;
    //Actions.WithCommandByName<UiCommand>(nameof(ShowSetlistEditorPage), c => c.Enabled = !(cp is SetlistEditorPage));
    //Actions.WithCommandByName<UiCommand>(nameof(ShowPlayerPage), c => c.Enabled = !(cp is PlayerPage));
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
    await CurrentSetlist.Load(filename);
    await CurrentSetlist.UpdateFromReaper(Client);
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
    var fre = new FileRequestEventArgs
    {
      Type = FileRequestEventArgs.RequestType.SaveFile,
      SelectedPath = CurrentSetlist.Filename,
      Title = "Save from File",
      Filter = "Zuljeah Setlist (*.zuljeah)|*.zuljeah"
    };
    if (await UiSettings.DialogService.RequestFile(fre))
    {
      await CurrentSetlist.Save(fre.SelectedPath);
      if (CurrentPage != null)
        await CurrentPage.Refresh();
    }
  }

  [UiCommand(Caption = "Edit Setlist", Page = "Setlist", Image = "Change")]
  public async Task ShowSetlistEditorPage()
  {
    await AddPage(new SetlistEditorPage(this));
  }

  public async Task AddPage(IPage page)
  {
    Pages.Add(page);
    CurrentPage = page;
  }

  public async Task ClosePage(IPage page)
  {
    if (Pages.Contains(page)) Pages.Remove(page);
    CurrentPage = Pages.FirstOrDefault();
    if (CurrentPage != null)
      await CurrentPage.Activate();
  }

}