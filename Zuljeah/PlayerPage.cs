using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Data.Extensions;
using Eos.Mvvm;
using Eos.Mvvm.Attributes;
using Hsp.Reaper.ApiClient;

namespace Zuljeah;

internal class PlayerPage : AsyncItemsViewModelBase<SetlistItem>, IPage
{

  public string Title => "Player";

  public SetlistItem? CurrentItem
  {
    get => GetAutoFieldValue<SetlistItem>();
    private set => SetAutoFieldValue(value);
  }

  public SetlistItem? NextItem => GetNextItem(CurrentItem);

  public SetlistItem? LastItem
  {
    get => GetAutoFieldValue<SetlistItem>();
    private set
    {
      if (SetAutoFieldValue(value))
        RaisePropertyChanged(nameof(NextItem));
    }
  }

  private IHost Host { get; }

  public TransportInfo? Transport
  {
    get => GetAutoFieldValue<TransportInfo?>();
    private set => SetAutoFieldValue(value);
  }


  public PlayerPage(IHost host)
  {
    Host = host;
  }


  public async Task Activate()
  {
    await Refresh();
  }


  [UiCommand(Caption = "Play", Image = "Start")]
  public async Task Play()
  {
    await Host.Client.Play();
  }

  [UiCommand(Caption = "Pause", Image = "Pause")]
  public async Task Pause()
  {
    await Host.Client.TogglePause();
  }

  [UiCommand(Caption = "Stop", Image = "Stop")]
  public async Task Stop()
  {
    await Host.Client.Stop();
  }

  [UiCommand(Caption = "Reset", Image = "Reset")]
  public async Task Rest()
  {
    LastItem = null;
    await Host.Client.Stop();
  }

  [UiCommand(Caption = "Resynch REAPER", Image = "Reopen")]
  public async Task Resynch()
  {
    await Host.CurrentSetlist.UpdateFromReaper(Host.Client);
  }

  protected override async Task<IEnumerable<SetlistItem>> GetItems()
  {
    await Host.CurrentSetlist.UpdateFromReaper(Host.Client);
    return Host.CurrentSetlist.Items
      .Where(i => i.Enabled)
      .OrderBy(i => i.Sequence)
      .ToArray();
  }

  public async Task UpdateTransport(TransportInfo? tpi)
  {
    Transport = tpi;
    await Task.WhenAll(Items.ToArray().Select(i => i.UpdateTransport(tpi)));

    var currentItem = Items.FirstOrDefault(i => i.IsActive);
    if (currentItem != null && currentItem != LastItem)
      LastItem = currentItem;

    var oldCurrentItem = CurrentItem;
    CurrentItem = currentItem;
    if (oldCurrentItem != null && currentItem == null)
      await FinishPlaying(oldCurrentItem);
  }

  private async Task FinishPlaying(SetlistItem item)
  {
    await Host.Client.Stop();
    SelectedItem = GetNextItem(item);
  }

  private SetlistItem? GetNextItem(SetlistItem? item)
  {
    if (item == null) return null;
    var currIndex = Items.IndexOf(item);
    return Items.TryGetValue(currIndex + 1, out var nextItem) ? nextItem : null;
  }

  public async Task PlayItem(SetlistItem item)
  {
    await Stop();
    await Host.Client.GoToRegion(item.RegionId);
    if (item.StartDelay != null)
      await Task.Delay(item.StartDelay.Value);
    await Host.Client.Play();
  }

}