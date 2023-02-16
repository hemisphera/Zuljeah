using System;
using System.Threading.Tasks;

namespace Zuljeah;

public abstract class Action
{

  public Guid Id { get; }

  public string Name { get; }


  protected Action(Guid id, string name)
  {
    Id = id;
    Name = name;
  }


  public async Task Execute()
  {
    try
    {
      await Callback();
    }
    catch
    {
      // ignore
    }
  }


  protected abstract Task Callback();


  public override string ToString()
  {
    return Name;
  }

}