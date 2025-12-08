using System.Diagnostics;

public class GameLife
{
  private Terrain _terrain;
  private System.Windows.Forms.Timer _timer = new();

  public GameLife()
  {
    // _terrain = new Terrain(60, 50);
    // _terrain = new ScannerTerrainDecorator(new Terrain(60, 50));
    // _terrain = new StatisticsTerrainDecorator(new Terrain(60, 50));
    // _terrain = new ScannerTerrainDecorator(new StatisticsTerrainDecorator(new Terrain(60, 50)));
    _terrain = new StatisticsTerrainDecorator(new ScannerTerrainDecorator(new Terrain(80, 50)));

    _terrain.SetName("GameLife");
    _terrain.SetLocation(20, 100);
    _terrain.SetDefaultSize();

    _timer.Interval = 350;
    _timer.Tick += TimerTick;
  }

  public void Run()
  {
    _timer.Start();
    _terrain.Show();
    _terrain.DrawFeatures(_terrain.Controls);
    Application.Run();
  }

  private void TimerTick(object? sender, EventArgs e)
  {
    if (_terrain.OnPause) return;
    _terrain.UpdateField();
    _terrain.Invalidate();
  }
}
