using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eos.Mvvm;
using Eos.Mvvm.Attributes;
using Hsp.Reaper.ApiClient;
using Microsoft.Extensions.DependencyInjection;

namespace Zuljeah;

public class SetlistEditorPage : AsyncItemsViewModelBase<SetlistItem>, IPage
{

  public string Title => "Setlist";

  private Setlist Setlist { get; }


  public SetlistEditorPage()
  {
    Setlist = App.Services.GetRequiredService<Setlist>();
  }


  protected override async Task<IEnumerable<SetlistItem>> GetItems()
  {
    return await Task.FromResult(Setlist.Items);
  }


  public async Task Activate()
  {
    await Setlist.UpdateFromReaper();
    await Refresh();
  }

  [UiCommand(Caption = "Delete", Page = "Editor", Group = "Edit", Image = "Delete")]
  public async Task RemoveItem()
  {
    var item = SelectedItem;
    if (item != null)
    {
      Setlist.Items.Remove(item);
      await Refresh();
    }
  }

  [UiCommand(Caption = "Add from REAPER", Page = "Editor", Group = "Edit", Image = "Import")]
  public async Task AddNewRegions()
  {
    var client = App.Services.GetRequiredService<ReaperApiClient>();
    var existingRegions = Setlist.Items.Select(i => i.RegionId).ToArray();
    var reaperRegions = await client.ListRegions();
    foreach (var reaperRegion in reaperRegions.OrderBy(o => o.Start))
    {
      if (existingRegions.Contains(reaperRegion.RegionId)) continue;
      var si = new SetlistItem
      {
        RegionId = reaperRegion.RegionId,
        Enabled = true
      };
      si.LoadRegion(reaperRegion);
      Setlist.Items.Add(si);
      await Refresh();
    }
  }

  [UiCommand(Caption = "Close", Page = "Editor", Group = "Edit", Image = "Close")]
  public async Task ClosePage()
  {
    await App.MainVmInstance.ClosePage(this);
  }

}