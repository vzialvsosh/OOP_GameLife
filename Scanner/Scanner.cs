public class StablePatternEvetnArgs : EventArgs
{
  public bool[,] Component { get; private set; }
  public Scanner.Frame Frame { get; private set; }

  public StablePatternEvetnArgs(bool[,] component, Scanner.Frame frame)
  {
    Component = component;
    Frame = frame;
  }
}

public static class Scanner
{
  public static Patterns patterns = new Patterns();
  public struct Frame
  {
    public int minX;
    public int maxX;
    public int minY;
    public int maxY;
  }
  public static event EventHandler<StablePatternEvetnArgs>? StablePatternDetected;

  public static bool[,] Scan(Terrain terrain)
  {
    Terrain copyTerrain = terrain.GetCopy();
    return AddColonies(GetComponents(copyTerrain), terrain.Colonies);
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

  private static bool[,] GetComponents(Terrain terrain)
  {
    bool[,] components = new bool[terrain.Width, terrain.Height];
    bool[,] component;
    Frame frame;
    for (int i = 0; i < terrain.Width; ++i)
    {
      for (int j = 0; j < terrain.Height; ++j)
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
    bool[,] component = new bool[terrain.Width, terrain.Height];
    Queue<Tuple<int, int>> queue = new Queue<Tuple<int, int>>();
    Frame frame = new Frame
    {
      minX = terrain.Width,
      maxX = 0,
      minY = terrain.Height,
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
    bool[,] newArray = new bool[component.GetLength(0), component.GetLength(1)];
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
    // return Util.ArraysEqual(component, Util.GetNextPhase(component));
  }

  // private static Colony CreateColony(bool[,] component)
  // {
  //   Colony colony = new();
  //   for (int i = 0; i < component.)
  // }
}
