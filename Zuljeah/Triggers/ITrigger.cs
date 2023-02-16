using System;

namespace Zuljeah;

public interface ITrigger : IEquatable<ITrigger>
{

  string Serialize();

  void Deserialize(string data);

}