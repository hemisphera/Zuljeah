using System.Threading.Tasks;

namespace Zuljeah;

public interface IPage
{

  public string Title { get; }

  public string Icon { get; }


  Task Activate();

  Task Refresh();

  Task Deactivate();

}