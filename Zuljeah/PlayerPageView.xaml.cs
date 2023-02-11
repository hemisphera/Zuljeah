using System;
using System.Windows.Controls;

namespace WpfApp1;

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