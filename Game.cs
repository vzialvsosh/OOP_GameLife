public class GameLife
{
  private Terrain _terrain;
  // private Terrain _favoritesFormTerrain;
  private LifeFrom _lifeForm;
  private FavoritesForm _favoritesForm;
  private System.Windows.Forms.Timer _timer = new();

  public GameLife()
  {
    _terrain = new Terrain(50, 50);
    _lifeForm = new LifeFrom(_terrain);
    Scanner.StablePatternDetected += _terrain.OrganizeColony;

    _favoritesForm = new FavoritesForm(_terrain);
    _favoritesForm.ChangeMask(Scanner.Scan(_terrain));

    _timer.Interval = 350;
    _timer.Tick += TimerTick;
  }

  public void Run()
  {
    _timer.Start();

    _lifeForm.Show();
    _favoritesForm.Show();
    Application.Run();
  }

  private void TimerTick(object? sender, EventArgs e)
  {
    _terrain.UpdateField();
    // Scanner.ScanTo(_terrain, _favoritesFormTerrain);
    _favoritesForm.ChangeMask(Scanner.Scan(_terrain));
    _lifeForm.Invalidate();
    _favoritesForm.Invalidate();
  }
}
