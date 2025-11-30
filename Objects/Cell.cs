public class Cell
{
  private Terrain _terrain;
  public Colony? Colony;// { get; private set; }

  // public enum CellColor
  // {
  //   White,
  //   Black
  // }
  public int I { get; private set; }
  public int J { get; private set; }
  public bool IsAlive { get; private set; }
  // public CellColor Color { get; private set; } = CellColor.White;

  public Cell(bool alive, Terrain terrain, int i, int j)
  {
    IsAlive = alive;
    _terrain = terrain;
    I = i;
    J = j;
  }

  public Cell GetCopy(Terrain terrain)
  {
    Cell cell = new(IsAlive, terrain, I, J);
    // cell.SetColony(Colony);
    cell.Colony = Colony;
    return cell;
  }

  public bool IsBlack() => IsAlive && Colony != null;

  public bool IsWhite() => IsAlive && Colony == null;

  // public bool WillBeAlive()
  // {
  //   int cntAlive = WhiteNeighboursCount() + BlackNeighboursCount();
  //   if (IsAlive && (cntAlive < 2 || cntAlive > 3)) return false;
  //   else if (!IsAlive && cntAlive == 3) return true;
  //   return IsAlive;
  // }

  public Cell GenerateNextCell()
  {
    int cntBlack = BlackNeighboursCount();
    int cntWhite = WhiteNeighboursCount();
    Cell cell = GetCellAfterDominating(cntBlack, cntWhite);
    if (cell.IsBlack())
    {
      if (cntBlack < 2 || cntBlack > 3)
      {
        cell.Kill();
        cell.SetColony(null);
      }
      return cell;
    }
    if (cell.IsWhite())
    {
      if (cntWhite < 2 || cntWhite > 3) cell.Kill();
      return cell;
    }
    if (cntWhite == 3) cell.MakeAlive();
    return cell;
  }
  
  private Cell GetCellAfterDominating(int cntBlack, int cntWhite)
  {
    Cell cell = new(IsAlive, _terrain, I, J);
    if (IsBlack())
    {
      // if (cntWhite > cntBlack + 1) Colony!.RemoveMember(this);
      // else Colony!.ReplaceMember(this, cell);
      if (cntWhite <= cntBlack + 1)
      {
        // Colony!.AddNextMember(cell);
        cell.SetColony(Colony);
      }
      return cell;
    }
    if (IsWhite())
    {
      if (cntWhite + 1 < cntBlack)
      {
        cell.AddToNeighboursColony();
      }
      return cell;
    }
    return cell;
  }

  public int WhiteNeighboursCount()
  {
    int cnt = _terrain.Field[I, J].IsWhite() ? -1 : 0;
    for (int x = I - 1; x <= I + 1; ++x)
    {
      for (int y = J - 1; y <= J + 1; ++y)
      {
        if (_terrain.IsOnField(x, y) && _terrain.Field[x, y].IsWhite()) ++cnt;
      }
    }
    return cnt;
  }

  public int BlackNeighboursCount()
  {
    int cnt = _terrain.Field[I, J].IsBlack() ? -1 : 0;
    for (int x = I - 1; x <= I + 1; ++x)
    {
      for (int y = J - 1; y <= J + 1; ++y)
      {
        if (_terrain.IsOnField(x, y) && _terrain.Field[x, y].IsBlack()) ++cnt;
      }
    }
    return cnt;
  }

  public void MakeAlive()
  {
    IsAlive = true;
  }

  public void Kill()
  {
    IsAlive = false;
    // SetColony(null);
    // MakeColonyNull();
  }

  public void SetColony(Colony? colony)
  {
    Colony?.RemoveNextMember(this);
    colony?.AddNextMember(this);
    Colony = colony;
  }

  public void MakeColonyNull()
  {
    Colony = null;
  }

  public void AddToNeighboursColony()
  {
    for (int x = I - 1; x <= I + 1; ++x)
    {
      for (int y = J - 1; y <= J + 1; ++y)
      {
        if (_terrain.IsOnField(x, y) && _terrain.Field[x, y].IsBlack())
        {
          _terrain.Field[x, y].AddToColony(this);
          return;
        }
      }
    }
  }

  public void AddToColony(Cell cell)
  {
    // Colony?.AddNextMember(cell);
    cell.SetColony(Colony);
  }
}
