using Reaper.Api.Client;

namespace WpfApp1;

public interface IHost
{

  Setlist CurrentSetlist { get; }

  ReaperApiClient Client { get; }

}