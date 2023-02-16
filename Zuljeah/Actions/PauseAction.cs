using System;
using System.Threading.Tasks;
using Hsp.Reaper.ApiClient;
using Microsoft.Extensions.DependencyInjection;

namespace Zuljeah;

public class PauseAction : Action
{

  public PauseAction() 
    : base(Guid.Parse("{46860DB8-275D-4CC0-92E5-8BA3269C0E93}"), "Pause")
  {
  }

  
  protected override async Task Callback()
  {
    var client = App.Services.GetRequiredService<ReaperApiClient>();
    await client.TogglePause();
  }

}