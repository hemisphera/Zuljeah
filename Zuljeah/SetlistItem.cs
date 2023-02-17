using System;
using System.Threading.Tasks;
using Eos.Mvvm;
using Hsp.Reaper.ApiClient;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Zuljeah;

[JsonObject(MemberSerialization.OptIn)]
public class SetlistItem : ObservableEntity
{

  private Region? Region { get; set; }

  [JsonProperty]
  public int RegionId { get; set; }

  [JsonProperty]
  public int Sequence { get; set; }

  [JsonProperty]
  public bool Enabled { get; set; }

  [JsonProperty]
  public TimeSpan? StartDelay { get; set; }



  public string? RegionName => Region?.Name;

  public TimeSpan? RegionStart => Region?.Start;

  public TimeSpan? RegionEnd => Region?.End;

  public TimeSpan? RegionDuration => Region?.Duration;

  public TimeSpan? RegionPosition
  {
    get
    {
      var r = Region;
      if (r == null) return null;
      return null;
    }
  }

  public TimeSpan? RegionTimeRemaining
  {
    get => GetAutoFieldValue<TimeSpan?>();
    private set => SetAutoFieldValue(value);
  }


  public double Percentage
  {
    get => GetAutoFieldValue<double>();
    private set => SetAutoFieldValue(value);
  }

  public bool IsActive
  {
    get => GetAutoFieldValue<bool>();
    private set => SetAutoFieldValue(value);
  }

  public bool IsSelected
  {
    get => GetAutoFieldValue<bool>();
    set
    {
      if (SetAutoFieldValue(value))
        App.Services.GetRequiredService<PlayerPage>().UpdateSelection(this);
    }
  }



  public async Task UpdateTransport(TimeSpan? time)
  {
    if (time == null || Region == null)
    {
      IsActive = false;
      Percentage = 0;
      return;
    }

    IsActive = time <= Region.End && time >= Region.Start;
    if (!IsActive)
    {
      Percentage = 0;
      RegionTimeRemaining = null;
    }
    else
    {
      Percentage = (time.Value - Region.Start).TotalMilliseconds / Region.Duration.TotalMilliseconds;
      RegionTimeRemaining = RegionDuration - (time - RegionStart);
    }

    await Task.CompletedTask;
  }


  public void LoadRegion(Region? region)
  {
    Region = region;
    RaisePropertyChangedAll();
  }


  public override string ToString()
  {
    return RegionName ?? $"<UnknownRegionId{RegionId}";
  }

}