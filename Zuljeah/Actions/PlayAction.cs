using System;
using System.Threading.Tasks;
using Hsp.Reaper.ApiClient;
using Microsoft.Extensions.DependencyInjection;

namespace Zuljeah;

public class PlayAction : Action
{

  public PlayAction()
    : base(new Guid("910FB16E-E94D-4275-AB39-A7267265F361"), "Play")
  {
  }


  protected override async Task Callback()
  {
    var client = App.Services.GetRequiredService<ReaperApiClient>();
    await client.Stop();

    var player = App.Services.GetRequiredService<PlayerPage>();
    var item = player.SelectedItem;
    if (item == null) return;

    await client.GoToRegion(item.RegionId);
    if (item.StartDelay != null)
      await Task.Delay(item.StartDelay.Value);
    await client.Play();
  }

}