using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Zuljeah;

public class MoveNextAction : Action
{

  public MoveNextAction() 
    : base(Guid.Parse("{731178E8-8D5A-427C-848E-B12B1874B984}"), "Move Next")
  {
  }

  protected override async Task Callback()
  {
    var player = App.Services.GetRequiredService<PlayerPage>();
    var newItem = player.GetNextItem(player.SelectedItem);
    player.SelectedItem = newItem;
    await Task.CompletedTask;
  }

}