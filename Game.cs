public class GameLife
{
  private Terrain _lifeFormTerrain;
  private Terrain _favoritesFormTerrain;
  private LifeFrom _lifeForm;
  private FavoritesForm _favoritesForm;
  private System.Windows.Forms.Timer _timer;

  public GameLife()
  {
    _lifeFormTerrain = new Terrain(50, 50);
    _lifeForm = new LifeFrom(_lifeFormTerrain);
    _favoritesFormTerrain = _lifeFormTerrain.GetCopy();
    _favoritesForm = new FavoritesForm(_favoritesFormTerrain);
    Scanner.ScanTo(_lifeFormTerrain, _favoritesFormTerrain);
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
    _lifeFormTerrain.UpdateField();
    Scanner.ScanTo(_lifeFormTerrain, _favoritesFormTerrain);
    _lifeForm.Invalidate();
    _favoritesForm.Invalidate();
  }
}
