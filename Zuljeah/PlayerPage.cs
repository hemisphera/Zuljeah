using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Data.Extensions;
using Eos.Mvvm;
using Eos.Mvvm.Attributes;
using Hsp.Reaper.ApiClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Zuljeah;

public class PlayerPage : AsyncItemsViewModelBase<SetlistItem>, IPage
{

  private ReaperApiClient Client { get; }

  private ActionContainer Actions { get; }

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
    set
    {
      if (SetAutoFieldValue(value))
        RaisePropertyChanged(nameof(NextItem));
    }
  }

  public TransportPlayState PlayState
  {
    get => GetAutoFieldValue<TransportPlayState>();
    private set => SetAutoFieldValue(value);
  }

  public TimeSpan? TimePosition
  {
    get => GetAutoFieldValue<TimeSpan?>();
    private set => SetAutoFieldValue(value);
  }

  public int BeatsPerMeasure { get; private set; }

  public int CurrBeats { get; private set; }

  public string BeatString => GetBeatString(CurrBeats, BeatsPerMeasure);

  private ILogger<PlayerPage> Logger { get; }



  public PlayerPage(ReaperApiClient client, ActionContainer actions, ILogger<PlayerPage> logger)
  {
    Client = client;
    Actions = actions;
    Logger = logger;
  }



  public async Task Activate()
  {
    await Refresh();
  }


  [UiCommand(Caption = "Play", Image = "Start")]
  public async Task Play()
  {
    await Actions.Play.Execute();
  }

  [UiCommand(Caption = "Pause", Image = "Pause")]
  public async Task Pause()
  {
    await Actions.Pause.Execute();
  }

  [UiCommand(Caption = "Stop", Image = "Stop")]
  public async Task Stop()
  {
    await Actions.Stop.Execute();
  }

  [UiCommand(Caption = "Reset", Image = "Reset")]
  public async Task Reset()
  {
    await Actions.ResetAction.Execute();
  }

  [UiCommand(Caption = "Resynch REAPER", Image = "Reopen")]
  public async Task Resynch()
  {
    await Actions.ResynchAction.Execute();
  }

  protected override async Task<IEnumerable<SetlistItem>> GetItems()
  {
    var setlist = App.Services.GetRequiredService<Setlist>();
    await setlist.UpdateFromReaper();
    return setlist.Items
      .Where(i => i.Enabled)
      .OrderBy(i => i.Sequence)
      .ToArray();
  }

  private static string GetBeatString(int currBeats, int beatsPerMeasure)
  {
    var r = "";
    for (var i = 0; i < beatsPerMeasure; i++)
      r += i + 1 <= currBeats ? "O" : ".";
    return r;
  }

  internal async Task UpdateTransport(BeatPosInfo? beatPosInfo)
  {
    PlayState = beatPosInfo?.PlayState ?? TransportPlayState.Stopped;
    TimePosition = beatPosInfo?.TimePosition;
    await Task.WhenAll(Items.ToArray().Select(i => i.UpdateTransport(beatPosInfo?.TimePosition)));

    var currentItem = Items.FirstOrDefault(i => i.IsActive);
    if (currentItem != null && currentItem != LastItem)
      LastItem = currentItem;

    BeatsPerMeasure = beatPosInfo?.Numerator ?? 0;
    CurrBeats = (int)(beatPosInfo?.BeatsInMeasure ?? 0.0) + 1;
    RaisePropertyChanged(nameof(BeatString));

    var oldCurrentItem = CurrentItem;
    CurrentItem = currentItem;
    if (oldCurrentItem != null && currentItem == null)
      await FinishPlaying(oldCurrentItem);
  }

  private async Task FinishPlaying(SetlistItem item)
  {
    if (item.AfterPlayback == AfterPlaybackAction.Pause)
      await Client.TogglePause();
    if (item.AfterPlayback == AfterPlaybackAction.Stop)
      await Client.Stop();

    await Actions.MoveNextAction.Execute();

    if (item.AfterPlayback == AfterPlaybackAction.Continue)
      await Actions.Play.Execute();
  }

  internal SetlistItem? GetNextItem(SetlistItem? item)
  {
    if (item == null) return null;
    var currIndex = Items.IndexOf(item);
    return Items.TryGetValue(currIndex + 1, out var nextItem) ? nextItem : null;
  }

  internal SetlistItem? GetPreviousItem(SetlistItem? item)
  {
    if (item == null) return null;
    var currIndex = Items.IndexOf(item);
    return Items.TryGetValue(currIndex - 1, out var nextItem) ? nextItem : null;
  }

  public async Task InvokeAction(ITrigger trigger)
  {
    Logger.LogDebug($"Received trigger: {trigger}");
    var bindings = App.Services.GetRequiredService<ActionBindingsEditor>();
    var binding = bindings.FirstOrDefault(b => b.Enabled && b.Trigger?.Equals(trigger) == true);
    if (binding != null)
    {
      Logger.LogDebug($"Running action '{binding.Action?.Name ?? String.Empty}' for trigger {trigger}");
      await binding.Action.Execute();
    }
  }

}