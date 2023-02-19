using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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

  public string ApplicationTitle => GetApplicationTitle();

  public StatusBroker Status => App.Services.GetRequiredService<StatusBroker>();


  public IPage[] Pages
  {
    get => GetAutoFieldValue<IPage[]>();
    private set => SetAutoFieldValue(value);
  }

  public IPage? CurrentPage
  {
    get => GetAutoFieldValue<IPage>();
    private set => SetAutoFieldValue(value);
  }

  public bool? CleanupComplete { get; private set; }


  public MainVm(ReaperApiClient client)
  {
    Client = client;
    ActionRoot = new UiCommandCategory();
    Pages = Array.Empty<IPage>();

    Task.Run(async () =>
    {
      await client.RegisterCallback("Tick", TimeSpan.FromMilliseconds(125), UpdateTransportInfo);
    });
  }


  public async Task Initialize(string[] args)
  {
    var pages = new List<IPage>();
    pages.Add(App.Services.GetRequiredService<PlayerPage>());

    var bindings = App.Services.GetRequiredService<ActionBindingsEditor>();
    await bindings.Load();
    pages.Add(bindings);

    var setlist = App.Services.GetRequiredService<SetlistEditorPage>();
    pages.Add(setlist);

    var filename = args.FirstOrDefault();
    if (!String.IsNullOrEmpty(filename))
      await LoadSetlist(filename);

    Pages = pages.ToArray();
    CurrentPage = Pages.FirstOrDefault();
  }


  private void UpdateActions()
  {
    var container = new CommandContainer();
    if (CurrentPage != null)
      container.CollectFrom(CurrentPage);
    container.CollectFrom(this);

    Dispatch(() =>
    {
      ActionRoot.Pages.Clear();
      ActionRoot.Pages.AddRange(container);
    });
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


  public async Task ChangePage(IPage? newPage)
  {
    var oldPage = CurrentPage;
    if (oldPage != null) await oldPage.Deactivate();

    CurrentPage = newPage;
    if (newPage != null)
    {
      await newPage.Activate();
      await newPage.Refresh();
    }

    UpdateActions();
  }


  private string GetApplicationTitle()
  {
    const string title = "Zuljeah Reborn";
    var parts = new List<string>
    {
      title
    };

    var setlist = App.Services.GetRequiredService<Setlist>();
    if (!string.IsNullOrEmpty(setlist.Filename))
      parts.Add(Path.GetFileName(setlist.Filename));

    var receiver = App.Services.GetRequiredService<MidiReceiver>();
    if (!String.IsNullOrEmpty(receiver.DeviceName))
      parts.Add(receiver.DeviceName);

    return String.Join(" | ", parts);
  }

  public void UpdateApplicationTitle()
  {
    RaisePropertyChanged(nameof(ApplicationTitle));
  }

  public async Task Cleanup()
  {
    CleanupComplete = false;

    await ChangePage(null);

    var client = App.Services.GetRequiredService<ReaperApiClient>();
    await client.DisposeAsync();
    var receiver = App.Services.GetRequiredService<MidiReceiver>();
    await receiver.DisposeAsync();

    CleanupComplete = true;
  }

}