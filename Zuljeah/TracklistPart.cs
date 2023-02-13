using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eos.Mvvm;
using Hsp.Reaper.ApiClient;

namespace Zuljeah;

internal class TracklistPart : AsyncItemsViewModelBase<Track>
{

  private IHost Host { get; }


  public TracklistPart(IHost host)
  {
    Host = host;
    Host.Client.RegisterCallback(TimeSpan.FromMilliseconds(250), Refresh);
  }


  protected override async Task<IEnumerable<Track>> GetItems()
  {
    var tracks = await Host.Client.ListTracks();
    return tracks;
  }

}