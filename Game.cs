public class GameLife
{
  private Terrain _terrain;
  private System.Windows.Forms.Timer _timer = new();

  public GameLife()
  {
    _terrain = new ScannerTerrainDecorator(new StatisticsTerrainDecorator(new Terrain(50, 50)));
    // _terrain = new StatisticsTerrainDecorator(new ScannerTerrainDecorator(new Terrain(50, 50)));

    _terrain.SetName("GameLife");
    _terrain.SetLocation(20, 100);
    _terrain.SetSize(750, 600);

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
    _terrain.UpdateField();
    _terrain.Invalidate();
  }
}
