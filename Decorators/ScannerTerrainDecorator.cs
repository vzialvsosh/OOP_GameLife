public class StablePatternEvetnArgs : EventArgs
{
  public bool[,] Component { get; private set; }
  public ScannerTerrainDecorator.Frame Frame { get; private set; }

  public StablePatternEvetnArgs(bool[,] component, ScannerTerrainDecorator.Frame frame)
  {
    Component = component;
    Frame = frame;
  }
}

public class ScannerTerrainDecorator : TerrainDecorator
{
  private bool[,] _mask;
  public bool IgnoreMask { get; private set; } = true;

  public ScannerTerrainDecorator(Terrain terrain) : base(terrain)
  {
    StablePatternDetected += _terrain.OrganizeColony;
    ClearMask();
  }

  public void ChangeMask(bool[,] mask)
  {
    if (mask.GetLength(0) != TerrainWidth || mask.GetLength(1) != TerrainHeight)
      throw new InvalidOperationException("Mask doesn't fit the terrain");
    _mask = mask;
  }

  public void ClearMask()
  {
    _mask = new bool[TerrainWidth, TerrainHeight];
    for (int i = 0; i < TerrainWidth; ++i)
    {
      for (int j = 0; j < TerrainHeight; ++j)
      {
        _mask[i, j] = true;
      }
    }
  }

  public static Patterns patterns = new Patterns();
  public struct Frame
  {
    public int minX;
    public int maxX;
    public int minY;
    public int maxY;
  }
  public event EventHandler<StablePatternEvetnArgs>? StablePatternDetected;

  public void Scan()
  {
    Terrain copyTerrain = _terrain.GetCopy();
    _mask = AddColonies(GetComponents(copyTerrain), _terrain.Colonies);
  }

  private static bool[,] AddColonies(bool[,] components, HashSet<Colony> colonies)
  {
    foreach (Colony colony in colonies)
    {
      foreach (Cell cell in colony.Members)
      {
        components[cell.I, cell.J] = true;
      }
    }
    return components;
  }

  private bool[,] GetComponents(Terrain terrain)
  {
    bool[,] components = new bool[terrain.TerrainWidth, terrain.TerrainHeight];
    bool[,] component;
    Frame frame;
    for (int i = 0; i < terrain.TerrainWidth; ++i)
    {
      for (int j = 0; j < terrain.TerrainHeight; ++j)
      {
        if (terrain.Field[i, j].IsWhite())
        {
          (component, frame) = CutComponent(terrain, i, j);
          if (MatchesPattern(component, i, j)) AddComponent(components, component, frame);
          if (IsStable(component, frame))
          {
            AddComponent(components, component, frame);
            StablePatternDetected?.Invoke(null, new StablePatternEvetnArgs(component, frame));
          }
        }
      }
    }
    return components;
  }

  private static (bool[,] component, Frame frame) CutComponent(Terrain terrain, int i, int j)
  {
    bool[,] component = new bool[terrain.TerrainWidth, terrain.TerrainHeight];
    Queue<Tuple<int, int>> queue = new Queue<Tuple<int, int>>();
    Frame frame = new Frame
    {
      minX = terrain.TerrainWidth,
      maxX = 0,
      minY = terrain.TerrainHeight,
      maxY = 0
    };

    queue.Enqueue(new Tuple<int, int>(i, j));
    terrain.Field[i, j].Kill();
    component[i, j] = true;
    frame = UpdateFrame(frame, i, j);

    while (queue.Count > 0)
    {
      (i, j) = queue.Dequeue();
      for (int x = i - 1; x <= i + 1; ++x)
      {
        for (int y = j - 1; y <= j + 1; ++y)
        {
          if (!terrain.IsOnField(x, y)) continue;
          if (!terrain.Field[x, y].IsWhite()) continue;
          queue.Enqueue(new Tuple<int, int>(x, y));
          component[x, y] = true;
          terrain.Field[x, y].Kill();
          frame = UpdateFrame(frame, x, y);
        }
      }
    }
    return (component, frame);
  }

  private static Frame UpdateFrame(Frame frame, int i, int j)
  {
    frame.maxX = Math.Max(i, frame.maxX);
    frame.minX = Math.Min(i, frame.minX);
    frame.maxY = Math.Max(j, frame.maxY);
    frame.minY = Math.Min(j, frame.minY);
    return frame;
  }

  private static bool[,] AddComponent(bool[,] components, bool[,] component, Frame frame)
  {
    if (components.GetLength(0) != component.GetLength(0)) throw new Exception("Component's size doesn't fit");
    if (components.GetLength(1) != component.GetLength(1)) throw new Exception("Component's size doesn't fit");
    for (int i = frame.minX; i <= frame.maxX; ++i)
    {
      for (int j = frame.minY; j <= frame.maxY; ++j)
      {
        components[i, j] |= component[i, j];
      }
    }
    return components;
  }

  private static bool MatchesPattern(bool[,] component, int i, int j)
  {
    foreach (Patterns.Pattern pattern in patterns.AllPatterns)
    {
      if (Matches(pattern, component, i, j)) return true;
    }
    return false;
  }

  private static bool Matches(Patterns.Pattern pattern, bool[,] component, int i, int j)
  {
    int deltaI = i - pattern.root[0];
    int deltaJ = j - pattern.root[1];
    if (deltaI < 0) return false;
    if (deltaJ < 0) return false;
    if (deltaI + pattern.array.GetLength(0) > component.GetLength(0)) return false;
    if (deltaJ + pattern.array.GetLength(1) > component.GetLength(1)) return false;

    for (int x = 0; x < pattern.array.GetLength(0); ++x)
    {
      for (int y = 0; y < pattern.array.GetLength(1); ++y)
      {
        if (!component[x + deltaI, y + deltaJ] == pattern.array[x, y]) return false;
      }
    }
    return CheckSides(pattern, component, deltaI, deltaJ);
  }

  private static bool CheckSides(Patterns.Pattern pattern, bool[,] component, int deltaI, int deltaJ)
  {
    for (int x = deltaI - 1; x <= deltaI + pattern.array.GetLength(0); ++x)
    {
      if (Util.IsOnArray(component, x, deltaJ - 1) && component[x, deltaJ - 1]) return false;
    }
    for (int x = deltaI - 1; x <= deltaI + pattern.array.GetLength(0); ++x)
    {
      if (Util.IsOnArray(component, x, deltaJ + pattern.array.GetLength(1)) &&
        component[x, deltaJ + pattern.array.GetLength(1)]) return false;
    }

    for (int y = deltaJ - 1; y <= deltaJ + pattern.array.GetLength(1); ++y)
    {
      if (Util.IsOnArray(component, deltaI - 1, y) && component[deltaI - 1, y]) return false;
    }
    for (int y = deltaJ - 1; y <= deltaJ + pattern.array.GetLength(1); ++y)
    {
      if (Util.IsOnArray(component, deltaI + pattern.array.GetLength(0), y) &&
        component[deltaI + pattern.array.GetLength(0), y]) return false;
    }
    return true;
  }

  private static bool IsStable(bool[,] component, Frame frame)
  {
    for (int i = frame.minX - 1; i <= frame.maxX + 1; ++i)
    {
      for (int j = frame.minY; j <= frame.maxY + 1; ++j)
      {
        if (!Util.IsOnArray(component, i, j)) continue;

        int cnt = Util.GetCountNeighbours(component, i, j);
        if (component[i, j] && (cnt < 2 || cnt > 3)) return false;
        else if (!component[i, j] && cnt == 3) return false;
      }
    }
    return true;
  }

  // protected override void Draw(object? sender, PaintEventArgs e)
  // {
  //   UpdateField();
  //   DrawTerrain(e);
  //   DrawFeatures(Controls);
  // }

  public override void DrawTerrain(PaintEventArgs e, bool[,]? mask, bool? ignoreMask)
  {
    Scan();
    _terrain.DrawTerrain(e, _mask, IgnoreMask);
  }

  public void DrawTerrainWithoutScan(PaintEventArgs e)
  {
    _terrain.DrawTerrain(e, _mask, IgnoreMask);
  }

  public override void DrawFeatures(Control.ControlCollection controls)
  {
    _terrain.DrawFeatures(controls);

    CheckBox checkBoxOnlyFavourites = new();
    checkBoxOnlyFavourites.Size = new Size(200, 50);
    checkBoxOnlyFavourites.Text = $"Show only favorites";
    checkBoxOnlyFavourites.Location = new Point(_cellSize * TerrainWidth + 10, 50);

    controls.Add(checkBoxOnlyFavourites);
    // checkBoxOnlyFavourites.CheckedChanged += (sender, e) => SetIgnoreMask(!checkBoxOnlyFavourites.Checked);
    checkBoxOnlyFavourites.CheckedChanged += (sender, e) => IgnoreMask = !checkBoxOnlyFavourites.Checked;
  }
}
