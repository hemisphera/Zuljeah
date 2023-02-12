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

  private void RegionControl_OnPlayStarted(object? sender, SetlistItem e)
  {
    if (DataContext is not PlayerPage dc) return; 
    dc.PlayItem(e);
  }

}