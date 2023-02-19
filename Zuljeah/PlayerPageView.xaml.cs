using System.Windows;
using System.Windows.Controls;

namespace Zuljeah;

/// <summary>
/// Interaction logic for PlayerPageView.xaml
/// </summary>
public partial class PlayerPageView : UserControl
{

  public PlayerPageView()
  {
    InitializeComponent();
  }

  private void RegionControl_OnSelectedChanged(object? sender, bool e)
  {
    Dispatcher.Invoke(() =>
    {
      if (e)
        (sender as FrameworkElement)?.BringIntoView();
    });
  }

}