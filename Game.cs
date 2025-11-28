public class GameLife
{
  private Terrain _lifeFormTerrain;
  private Terrain _favoritesFormTerrain;
  private LifeFrom _lifeForm;
  private FavoritesForm _favoritesForm;
  private System.Windows.Forms.Timer _timer = new();
  private HashSet<Colony> _colonies = new();

  public GameLife()
  {
    _lifeFormTerrain = new Terrain(50, 50);
    _lifeForm = new LifeFrom(_lifeFormTerrain);
    _favoritesFormTerrain = _lifeFormTerrain.GetCopy();
    _favoritesForm = new FavoritesForm(_favoritesFormTerrain);
    Scanner.ScanTo(_lifeFormTerrain, _favoritesFormTerrain);

    _timer.Interval = 500;
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
    _lifeFormTerrain.UpdateField();
    Scanner.ScanTo(_lifeFormTerrain, _favoritesFormTerrain);
    _lifeForm.Invalidate();
    _favoritesForm.Invalidate();
  }

  public void AddColony(Colony colony)
  {
    _colonies.Add(colony);
  }
  
  public void RemoveColony(Colony colony)
  {
    _colonies.Remove(colony);
  }
}
