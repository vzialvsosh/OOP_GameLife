public class GameLife
{
  private Terrain _terrain;
  private LifeFrom _lifeForm;
  private LifeFrom _favoritesForm;
  private System.Windows.Forms.Timer _timer;

  public GameLife()
  {
    _terrain = new Terrain(10, 10);
    _lifeForm = new LifeFrom(_terrain);
    _favoritesForm = new LifeFrom(Scanner.Scan(_terrain));

    _timer = new System.Windows.Forms.Timer();
    _timer.Interval = 3000;
    _timer.Tick += TimerTick;
    _timer.Start();
  }

  public void Run()
  {
    // _lifeForm.Show();
    _favoritesForm.Show();
    Application.Run(_lifeForm);
    // Application.Run(_favoritesForm);
  }

  private void TimerTick(object? sender, EventArgs e)
  {
    _terrain.UpdateField();
    _favoritesForm = new LifeFrom(Scanner.Scan(_terrain));
    _lifeForm.Invalidate();
    _favoritesForm.Invalidate();
  }
}
