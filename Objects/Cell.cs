public enum CellType
{
  White,
  Black,
  Empty
}

public record CellContext
{
  public CellType cellType;
  public int cntWhite;
  public int cntBlack;
  public int cntAlive;
}

public class Cell
{
  private Terrain _terrain;
  public Colony? Colony { get; private set; }

  public int I { get; private set; }
  public int J { get; private set; }
  public bool IsAlive { get; private set; }

  public bool IsBlack() => IsAlive && Colony != null;

  public bool IsWhite() => IsAlive && Colony == null;

  public Cell(bool alive, Terrain terrain, int i, int j)
  {
    IsAlive = alive;
    _terrain = terrain;
    I = i;
    J = j;
  }

  public Cell(ICellStrategyProvider strategies, Terrain terrain, int i, int j)
  {
    _terrain = terrain;
    I = i;
    J = j;
    CellType cellType = strategies.For(terrain.Field[i, j].GetCellType()).GetNext(GetCellContext());
    if (cellType != CellType.Empty)
    {
      IsAlive = true;
      if (cellType == CellType.Black)
      {
        AddToNeighboursColony();
      }
    }
  }

  public CellType GetCellType()
  {
    if (IsWhite()) return CellType.White;
    if (IsBlack()) return CellType.Black;
    return CellType.Empty;
  }

  public CellContext GetCellContext()
  {
    int cntW = WhiteNeighboursCount();
    int cntB = BlackNeighboursCount();
    return new CellContext {
      cellType = GetCellType(),
      cntWhite = cntW,
      cntBlack = cntB,
      cntAlive = cntW + cntB
    };
  }

  public Cell GetCopy(Terrain terrain)
  {
    Cell cell = new(IsAlive, terrain, I, J);
    cell.Colony = Colony;
    return cell;
  }

  // public Cell GenerateNextCell()
  // {
  //   int cntBlack = BlackNeighboursCount();
  //   int cntWhite = WhiteNeighboursCount();
  //   Cell cell = GetCellAfterDominating(cntBlack, cntWhite);
  //   if (cell.IsBlack())
  //   {
  //     if (cntBlack < 2 || cntBlack > 3)
  //     {
  //       cell.Kill();
  //       cell.SetColony(null);
  //     }
  //     return cell;
  //   }
  //   if (cell.IsWhite())
  //   {
  //     if (cntWhite < 2 || cntWhite > 3) cell.Kill();
  //     return cell;
  //   }
  //   if (cntWhite == 3) cell.MakeAlive();
  //   return cell;
  // }

  // private Cell GetCellAfterDominating(int cntBlack, int cntWhite)
  // {
  //   Cell cell = new(IsAlive, _terrain, I, J);
  //   if (IsBlack())
  //   {
  //     if (cntWhite <= cntBlack + 1)
  //     {
  //       cell.SetColony(Colony);
  //     }
  //     return cell;
  //   }
  //   if (IsWhite())
  //   {
  //     if (cntWhite + 1 < cntBlack)
  //     {
  //       cell.AddToNeighboursColony();
  //     }
  //     return cell;
  //   }
  //   return cell;
  // }

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
  }

  public void SetColony(Colony? colony)
  {
    Colony?.RemoveNextMember(this);
    colony?.AddNextMember(this);
    Colony = colony;
  }

  public void SetColonyManually(Colony? colony)
  {
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
    cell.SetColony(Colony);
  }
}
