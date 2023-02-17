using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.WindowsUI;
using Eos.Mvvm.Commands;
using Eos.Mvvm.EventArgs;
using Microsoft.Extensions.DependencyInjection;

namespace Zuljeah;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{

  public MainWindow()
  {
    InitializeComponent();
    DataContext = App.MainVmInstance;
  }

  private void RibbonPageGroup_ItemTemplateSelector_OnOnSelectTemplate(object sender, DataTemplateEventArgs e)
  {
    if (e.Item is not UiCommand act) return;

    var resource = act.Commands.Any()
      ? FindResource("ButtonListActionTemplate")
      : FindResource("ButtonActionTemplate");

    e.DataTemplate = resource as DataTemplate;
  }

  private async void MainWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
  {
    if (DataContext is not MainVm dc) return;
    if (dc.CurrentPage is not PlayerPage player) return;
    var handled = await player.InvokeAction(new KeyTrigger(e.Key));
    if (handled) e.Handled = true;
  }

  private async void HamburgerMenu_OnSelectedItemChanged(object? sender, HamburgerMenuSelectedItemChangedEventArgs e)
  {
    var main = App.Services.GetRequiredService<MainVm>();
    await main.ChangePage(e.NewItem as IPage);
  }

  private void MainWindow_OnClosing(object? sender, CancelEventArgs e)
  {
    var complete = App.MainVmInstance.CleanupComplete;
    e.Cancel = complete != true;

    if (App.MainVmInstance.CleanupComplete == false) return;

    if (App.MainVmInstance.CleanupComplete == null)
      App.MainVmInstance.Cleanup()
        .ContinueWith(async a =>
        {
          await Task.Delay(TimeSpan.FromSeconds(1));
          await Dispatcher.InvokeAsync(Close);
        });
  }

}