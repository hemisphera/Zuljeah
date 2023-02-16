using System;
using System.Threading.Tasks;
using DevExpress.Mvvm.POCO;
using Hsp.Reaper.ApiClient;

namespace Zuljeah;

public class ResetAction : Action
{

  public ResetAction()
    : base(Guid.Parse("{0D424ABB-6F0B-4239-9C58-B9F37709A2FE}"), "Reset")
  {
  }


  protected override async Task Callback()
  {
    var client = App.Services.GetRequiredService<ReaperApiClient>();
    var player = App.Services.GetRequiredService<PlayerPage>();
    player.LastItem = null;
    await client.Stop();
  }

}