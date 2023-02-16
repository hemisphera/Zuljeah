using System;
using System.Windows.Input;

namespace Zuljeah;

internal class KeyTrigger : ITrigger
{

  public Key Key { get; set; }


  public KeyTrigger()
  {
  }

  public KeyTrigger(Key key)
  {
    Key = key;
  }


  public string Serialize()
  {
    return $"{(int)Key}";
  }

  public void Deserialize(string data)
  {
    Key = (Key)int.Parse(data);
  }

  public bool Equals(ITrigger? other)
  {
    if (other is not KeyTrigger t2) return false;
    return t2.Key == Key;
  }

}