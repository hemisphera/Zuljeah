using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Zuljeah;

public class MovePreviousAction : Action
{

  public MovePreviousAction()
    : base(Guid.Parse("{E6B2E382-52E9-45BA-860E-BB219F4D61CE}"), "Move Previous")
  {
  }

  protected override async Task Callback()
  {
    var player = App.Services.GetRequiredService<PlayerPage>();
    var newItem = player.GetPreviousItem(player.SelectedItem);
    if (newItem != null)
      newItem.IsSelected = true;
    await Task.CompletedTask;
  }

}