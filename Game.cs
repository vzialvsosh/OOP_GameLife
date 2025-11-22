public class GameLife
{
  private Terrain _terrain;
  private LifeFrom _lifeForm;
  private FavoritesForm _favoritesForm;
  private System.Windows.Forms.Timer _timer;

  public GameLife()
  {
    _terrain = new Terrain(50, 50);
    _lifeForm = new LifeFrom(_terrain);
    _favoritesForm = new FavoritesForm(Scanner.Scan(_terrain));
  }

  public void Run()
  {
    _timer = new System.Windows.Forms.Timer();
    _timer.Interval = 500;
    _timer.Tick += TimerTick;
    _timer.Start();

    _lifeForm.Show();
    _favoritesForm.Show();
    Application.Run();
  }

  private void TimerTick(object? sender, EventArgs e)
  {
    _terrain.UpdateField();
    _favoritesForm._terrain = Scanner.Scan(_terrain);
    _lifeForm.Invalidate();
    _favoritesForm.Invalidate();
  }
}
