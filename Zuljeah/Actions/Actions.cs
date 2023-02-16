using System.Linq;

namespace Zuljeah;

public class ActionContainer
{

  public Action[] All { get; }

  private Action[] CollectActions()
  {
    return GetType()
      .GetProperties()
      .Select(p => p.GetValue(this) as Action)
      .Where(n => n != null).Select(i => i!)
      .ToArray();
  }


  public ActionContainer()
  {
    All = CollectActions();
  }



  public Action Play { get; } = new PlayAction();

  public Action Stop { get; } = new StopAction();

  public Action Pause { get; } = new PauseAction();

  public Action ResetAction { get; } = new ResetAction();

  public Action ResynchAction { get; } = new ResynchAction();

  public Action MoveNextAction { get; } = new MoveNextAction();

  public Action MovePreviousAction { get; } = new MovePreviousAction();

}