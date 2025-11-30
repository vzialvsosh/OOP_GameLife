public class Colony
{
  private static List<int[]> _directions = new List<int[]>
  {
    new int[]{ 1, 0 },
    new int[]{ -1, 0 },
    new int[]{ 0, 1 },
    new int[]{ 0, -1 }
  };

  private Terrain _terrain;
  private int[] _direction = _directions[0];
  // private bool IsMoving = true;


  // public int Count { get; private set; }

  public HashSet<Cell> NextMembers { get; private set; } = new();
  public HashSet<Cell> Members { get; private set; } = new();
  // public int[] Direction { get; private set; } = Directions[0];

  public Colony(Terrain terrain)
  {
    _terrain = terrain;
    ChangeDirection();
  }

  public void UpdateMembers()
  {
    // foreach (Cell cell in Members)
    // {
    //   cell.MakeColonyNull();
    //   // Members.Remove(cell);
    // }
    Members = NextMembers;
    NextMembers = new();
    if (Members.Count == 0)
    {
      _terrain.RemoveColony(this);
    }
  }

  public void Move()
  {
    if (Capturing())
    {
      ChangeDirection();
      return;
    }
    if (UniteWithNeighbourColony())
    {
      ChangeDirection();
      return;
    }
    NextMembers = new();
    foreach (Cell cell in Members)
    {
      int x = cell.I + _direction[0];
      int y = cell.J + _direction[1];
      if (!_terrain.IsOnField(x, y))
      {
        ChangeDirection();
        NextMembers = new();
        return;
      }
      AddNextMember(_terrain.Field[x, y]);
    }

    foreach (Cell cell in Members)
    {
      cell.Kill();
    }
    foreach (Cell cell in NextMembers)
    {
      cell.MakeAlive();
      cell.Colony = this;
    }
    UpdateMembers();
  }

  private bool UniteWithNeighbourColony()
  {
    bool united = false;
    foreach (Cell cell in Members)
    {
      for (int x = cell.I - 1; x <= cell.I + 1; ++x)
      {
        for (int y = cell.J - 1; y <= cell.J + 1; ++y)
        {
          if (_terrain.IsOnField(x, y) && _terrain.Field[x, y].IsBlack())
          {
            united |= Unite(_terrain.Field[x, y].Colony!);
          }
        }
      }
    }
    return united;
  }
  
  private bool Unite(Colony colony)
  {
    if (this == colony) return false;
    foreach (Cell cell in colony.Members)
    {
      // NextMembers.Add(cell);
      cell.SetColony(this);
    }
    foreach (Cell cell in Members)
    {
      AddNextMember(cell);
    }
    colony.Members = new();
    _terrain.RemoveColony(colony);
    UpdateMembers();
    return true;
  }
  
  private bool Capturing()
  {
    foreach (Cell cell in Members)
    {
      if (cell.WhiteNeighboursCount() > 0) return true;
    }
    return false;
  }

  public void ChangeDirection()
  {
    Random rnd = new Random();
    _direction = _directions[rnd.Next(0, 4)];
  }

  public void AddNextMember(Cell cell)
  {
    NextMembers.Add(cell);
  }

  public void RemoveNextMember(Cell cell)
  {
    if (NextMembers.Remove(cell)) cell.MakeColonyNull();
  }

  // public void ReplaceNextMember(Cell replacedCell, Cell newCell)
  // {
  //   RemoveNextMember(replacedCell);
  //   AddNextMember(newCell);
  //   newCell.SetColony(this);
  // }
}
