using System;
using Eos.Mvvm;
using System.Reflection;
using System.Windows;
using Eos.Mvvm.DataTemplates;
using Eos.Mvvm.EventArgs;

namespace WpfApp1;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
  protected override void OnStartup(StartupEventArgs e)
  {
    base.OnStartup(e);

    UiSettings.ViewLocator = new ViewLocator(Assembly.GetExecutingAssembly());
    UiSettings.DialogService = new BasicDialogService();

    MainVm.Instance.Initialize(e.Args);
  }

  private void TimespanNoMillisecondsConverter_OnOnConvert(object? sender, ConverterEventArgs e)
  {
    if (e.Value == null) return;
    var ts = (TimeSpan)e.Value;
    e.Result = TimeSpan.FromSeconds(Math.Round(ts.TotalSeconds, 0));
  }

}