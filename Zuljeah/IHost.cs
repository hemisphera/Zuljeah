using Hsp.Reaper.ApiClient;

namespace Zuljeah;

public interface IHost
{

  Setlist CurrentSetlist { get; }

  ReaperApiClient Client { get; }

}