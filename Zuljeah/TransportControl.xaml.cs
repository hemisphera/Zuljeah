using System;
using System.Windows.Controls;
using Eos.Mvvm.EventArgs;
using Reaper.Api.Client;

namespace WpfApp1;

/// <summary>
/// Interaction logic for TransportControl.xaml
/// </summary>
public partial class TransportControl : UserControl
{
  public TransportControl()
  {
    InitializeComponent();
  }

  private void TimeSpanConverter_OnOnConvert(object? sender, ConverterEventArgs e)
  {
    if (e.Value == null) return;
    var ti = (TransportInfo)e.Value;
    var newTs = TimeSpan.FromSeconds(Math.Round(ti.TimePosition.TotalSeconds, 0));
    e.Result = $"[{ti.State}] {newTs}";
  }

}