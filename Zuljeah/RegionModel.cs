/*
 * using System.Threading.Tasks;
using Eos.Mvvm;
using Eos.Mvvm.Attributes;
using Reaper.Api.Client;

namespace WpfApp1;

public class RegionModel : ObservableEntity
{

  private Region Region { get; }

  public string Name => Region.Name;

  public int RegionId => Region.RegionId;


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


  public RegionModel(Region region)
  {
    Region = region;
  }


  public void SetPlayhead(TransportInfo? playhead)
  {
    IsActive = playhead != null && playhead.TimePosition <= Region.End && playhead.TimePosition >= Region.Start;
    if (!IsActive)
    {
      Percentage = 0;
    }
    else
    {
      Percentage = (playhead!.TimePosition - Region.Start).TotalMilliseconds / Region.Duration.TotalMilliseconds;
    }
  }

  [UiCommand(Caption = "Play")]
  public async Task PlayRegion()
  {
    await MainVm.Instance.Play(this);
  }

}
*/