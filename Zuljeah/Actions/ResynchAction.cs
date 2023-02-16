using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Zuljeah;

public class ResynchAction : Action
{

  public ResynchAction()
    : base(Guid.Parse("{9617EAF3-3E65-450F-8BBD-23B7E327D3C4}"), "Resynch from REAPER")
  {
  }


  protected override async Task Callback()
  {
    var setlist = App.Services.GetRequiredService<Setlist>();
    await setlist.UpdateFromReaper();
  }

}