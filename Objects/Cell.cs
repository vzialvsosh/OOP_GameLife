public class Cell
{
  private Terrain _terrain;
  private Colony? _colony;

  // public enum CellColor
  // {
  //   White,
  //   Black
  // }
  public int i, j;
  public bool IsAlive { get; private set; }
  // public CellColor Color { get; private set; } = CellColor.White;

  public Cell(bool alive, Terrain terrain, int i, int j)
  {
    IsAlive = alive;
    _terrain = terrain;
    this.i = i;
    this.j = j;
  }

  public bool IsBlack() => IsAlive && _colony != null;

  public bool IsWhite() => IsAlive && _colony == null;

  public bool WillBeAlive()
  {
    int cnt = CellNeighboursCount();
    if (IsAlive && (cnt < 2 || cnt > 3)) return false;
    else if (!IsAlive && cnt == 3) return true;
    return IsAlive;
  }

  private int CellNeighboursCount()
  {
    int cnt = _terrain.Field[i, j].IsAlive ? -1 : 0;
    for (int x = i - 1; x <= i + 1; ++x)
    {
      for (int y = j - 1; y <= j + 1; ++y)
      {
        if (_terrain.IsOnField(x, y) && _terrain.Field[x, y].IsAlive) ++cnt;
      }
    }
    return cnt;
  }

  public void MakeIsAlive()
  {
    IsAlive = true;
  }

  public void Kill()
  {
    IsAlive = false;
  }

  public void SetColony(Colony colony)
  {
    _colony = colony;
  }
}
