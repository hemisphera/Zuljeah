using System.Threading.Tasks;
using Eos.Mvvm.Commands;

namespace WpfApp1;

public interface IPage
{

  public string Title { get; }

  //CommandContainer Actions { get; }

  Task Activate();

  Task Refresh();

}