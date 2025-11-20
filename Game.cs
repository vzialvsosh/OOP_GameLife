public class GameLife
{
  private Terrain _terrain;
  private LifeFrom _lifeForm;
  private System.Windows.Forms.Timer _timer;

  public GameLife()
  {
    _terrain = new Terrain(20, 20);
    _lifeForm = new LifeFrom(_terrain);

    _timer = new System.Windows.Forms.Timer();
    _timer.Interval = 300;
    _timer.Tick += TimerTick;
    _timer.Start();
  }

  public void Run()
  {
    Application.Run(_lifeForm);
  }

  private void TimerTick(object? sender, EventArgs e)
  {
    _terrain.UpdateField();
    _lifeForm.Invalidate();
  }
}
