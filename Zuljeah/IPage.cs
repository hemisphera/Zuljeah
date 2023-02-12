using System.Threading.Tasks;

namespace Zuljeah;

public interface IPage
{

  public string Title { get; }

  //CommandContainer Actions { get; }

  Task Activate();

  Task Refresh();

}