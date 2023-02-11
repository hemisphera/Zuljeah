using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eos.Mvvm;
using Eos.Mvvm.Attributes;
using Eos.Mvvm.Commands;
using Eos.Mvvm.EventArgs;

namespace WpfApp1;

public class SetlistEditorPage : AsyncItemsViewModelBase<SetlistItem>, IPage
{

  public string Title => "Setlist";

  //public CommandContainer Actions { get; } = new();

  private IHost Host { get; }


  public SetlistEditorPage(IHost host)
  {
    Host = host;
    //Actions.CollectFrom(this);
  }


  protected override async Task<IEnumerable<SetlistItem>> GetItems()
  {
    return await Task.FromResult(Host.CurrentSetlist.Items);
  }


  public async Task Activate()
  {
    await Host.CurrentSetlist.UpdateFromReaper(Host.Client);
    await Refresh();
  }

  [UiCommand(Caption = "Delete", Page = "Editor", Group = "Edit", Image = "Delete")]
  public async Task RemoveItem()
  {
    var item = SelectedItem;
    if (item != null)
    {
      Host.CurrentSetlist.Items.Remove(item);
      await Refresh();
    }
  }

  [UiCommand(Caption = "Add from REAPER", Page = "Editor", Group = "Edit", Image = "Import")]
  public async Task AddNewRegions()
  {
    var existingRegions = Host.CurrentSetlist.Items.Select(i => i.RegionId).ToArray();
    var reaperRegions = await Host.Client.ListRegions();
    foreach (var reaperRegion in reaperRegions.OrderBy(o => o.Start))
    {
      if (existingRegions.Contains(reaperRegion.RegionId)) continue;
      var si = new SetlistItem
      {
        RegionId = reaperRegion.RegionId,
        Enabled = true
      };
      si.LoadRegion(reaperRegion);
      Host.CurrentSetlist.Items.Add(si);
      await Refresh();
    }
  }

  [UiCommand(Caption = "Close", Page = "Editor", Group = "Edit", Image = "Close")]
  public async Task ClosePage()
  {
    await MainVm.Instance.ClosePage(this);
  }

}