using Hsp.Reaper.ApiClient;

namespace WpfApp1;

public interface IHost
{

  Setlist CurrentSetlist { get; }

  ReaperApiClient Client { get; }

}