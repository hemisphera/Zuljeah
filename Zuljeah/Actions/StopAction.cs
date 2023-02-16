using System;
using System.Threading.Tasks;
using Hsp.Reaper.ApiClient;
using Microsoft.Extensions.DependencyInjection;

namespace Zuljeah;

public class StopAction : Action
{

  public StopAction()
    : base(Guid.Parse("{EEE8589F-D8F9-4083-B6E1-E238254281CC}"), "Stop")
  {
  }


  protected override async Task Callback()
  {
    var client = App.Services.GetRequiredService<ReaperApiClient>();
    await client.Stop();
  }

}