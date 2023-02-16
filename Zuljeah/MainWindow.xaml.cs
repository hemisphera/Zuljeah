using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm.POCO;
using Eos.Mvvm.Commands;
using Eos.Mvvm.EventArgs;
using Hsp.Reaper.ApiClient;

namespace Zuljeah;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{

  private ReaperApiClient Client { get; } = new ReaperApiClient(new Uri("http://localhost:8080"));

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
    await player.InvokeAction(new KeyTrigger(e.Key));
  }

}