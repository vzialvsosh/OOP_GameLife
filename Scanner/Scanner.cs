public static class Scanner
{
  public static Patterns patterns = new Patterns();

  public static Terrain ScanTo(Terrain terrain, Terrain outputTerrain)
  {
    terrain.CopyTo(outputTerrain);
    outputTerrain.FillByArray(GetComponents(outputTerrain));
    return outputTerrain;
  }

  public static bool[,] GetComponents(Terrain terrain)
  {
    bool[,] components = new bool[terrain.Width, terrain.Height];
    bool[,] component;
    for (int i = 0; i < terrain.Width; ++i)
    {
      for (int j = 0; j < terrain.Height; ++j)
      {
        if (terrain.Field[i, j].IsWhite())
        {
          component = CutComponent(terrain, i, j);
          if (MatchesPattern(component, i, j)) AddComponent(components, component);
          // if (IsStable())
        }
      }
    }
    return components;
  }

  public static bool[,] CutComponent(Terrain terrain, int i, int j)
  {
    bool[,] component = new bool[terrain.Width, terrain.Height];
    Queue<Tuple<int, int>> queue = new Queue<Tuple<int, int>>();
    queue.Enqueue(new Tuple<int, int>(i, j));
    terrain.Field[i, j].Kill();
    component[i, j] = true;
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
        }
      }
    }
    return component;
  }

  private static bool[,] AddComponent(bool[,] components, bool[,] component)
  {
    if (components.GetLength(0) != component.GetLength(0)) throw new Exception("Component's size doesn't fit");
    if (components.GetLength(1) != component.GetLength(1)) throw new Exception("Component's size doesn't fit");
    for (int i = 0; i < components.GetLength(0); ++i)
    {
      for (int j = 0; j < components.GetLength(1); ++j)
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
      if (IsStable(component)) ;
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

  private static bool IsStable(bool[,] component)
  {
    return Util.ArraysEqual(component, Util.GetNextPhase(component));
  }

  // private static Colony CreateColony(bool[,] component)
  // {
  //   Colony colony = new();
  //   for (int i = 0; i < component.)
  // }
}
