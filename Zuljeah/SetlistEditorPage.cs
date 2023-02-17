using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eos.Mvvm;
using Eos.Mvvm.Attributes;
using Eos.Mvvm.EventArgs;
using Hsp.Reaper.ApiClient;
using Microsoft.Extensions.DependencyInjection;

namespace Zuljeah;

public class SetlistEditorPage : AsyncItemsViewModelBase<SetlistItem>, IPage
{

  public string Title => "Setlist";

  public string Icon => "List";

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

  [UiCommand(Caption = "Delete", Group = "Edit", Image = "Delete")]
  public async Task RemoveItem()
  {
    var item = SelectedItem;
    if (item != null)
    {
      Setlist.Items.Remove(item);
      await Refresh();
    }
  }

  [UiCommand(Caption = "Add from REAPER", Group = "Edit", Image = "Import")]
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


  [UiCommand(Caption = "Load", Group = "Load/Save", Image = "Import")]
  public async Task LoadFromFile()
  {
    var fre = new FileRequestEventArgs
    {
      Type = FileRequestEventArgs.RequestType.OpenFile,
      Title = "Load from File",
      Filter = "Zuljeah Setlist (*.zuljeah)|*.zuljeah"
    };
    if (await UiSettings.DialogService.RequestFile(fre))
    {
      await Setlist.Load(fre.SelectedPath);
      await Refresh();
    }
  }

  [UiCommand(Caption = "Save", Group = "Load/Save", Image = "Save")]
  public async Task SaveToFile()
  {
    var fre = new FileRequestEventArgs
    {
      Type = FileRequestEventArgs.RequestType.SaveFile,
      SelectedPath = Setlist.Filename,
      Title = "Save from File",
      Filter = "Zuljeah Setlist (*.zuljeah)|*.zuljeah"
    };
    if (await UiSettings.DialogService.RequestFile(fre))
    {
      await Setlist.Save(fre.SelectedPath);
      await Refresh();
    }
  }

  public async Task Deactivate()
  {
    if (!String.IsNullOrEmpty(Setlist.Filename))
      await Setlist.Save(Setlist.Filename);
  }

}