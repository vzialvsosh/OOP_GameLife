public static class Scanner
{
  private enum Pattern
  {
    Block,
    Blinker1,
    Blinker2,
    BeeHive1,
    BeeHive2
    // Glider,
  }
  private static Dictionary<Pattern, bool[,]> Patterns = new Dictionary<Pattern, bool[,]>
  {
    [Pattern.Block] = new bool[,] {
      { true, true },
      { true, true }
    },
    [Pattern.Blinker1] = new bool[,]
    {
      { true, true, true }
    },
    [Pattern.Blinker2] = new bool[,]
    {
      { true },
      { true },
      { true }
    },
    [Pattern.BeeHive1] = new bool[,]
    {
      { false, true, false },
      { true, false, true },
      { true, false, true },
      { false, true, false }
    },
    [Pattern.BeeHive2] = new bool[,]
    {
      { false, true, true, false },
      { true, false, false, true },
      { false, true, true, false }
    }
  };

  private static Dictionary<Pattern, int[]> Roots = new Dictionary<Pattern, int[]>
  {
    [Pattern.Block] = new int[] { 0, 0 },
    [Pattern.Blinker1] = new int[] { 0, 0 },
    [Pattern.Blinker2] = new int[] { 0, 0 },
    [Pattern.BeeHive1] = new int[] { 0, 1 },
    [Pattern.BeeHive2] = new int[] { 0, 1 }
  };

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
        if (terrain.Field[i, j].Alive)
        {
          component = CutComponent(terrain, i, j);
          if (MatchesPattern(component, i, j)) AddComponent(components, component);
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
          if (!terrain.Field[x, y].Alive) continue;
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
    foreach (Pattern pattern in Enum.GetValues<Pattern>())
    {
      if (Matches(pattern, component, i, j)) return true;
    }
    return false;
  }

  private static bool Matches(Pattern pattern, bool[,] component, int i, int j)
  {
    // Console.WriteLine($"{i}, {j}");
    int deltaI = i - Roots[pattern][0];
    int deltaJ = j - Roots[pattern][1];
    if (deltaI < 0) return false;
    if (deltaJ < 0) return false;
    if (deltaI + Patterns[pattern].GetLength(0) > component.GetLength(0)) return false;
    if (deltaJ + Patterns[pattern].GetLength(1) > component.GetLength(1)) return false;

    for (int x = 0; x < Patterns[pattern].GetLength(0); ++x)
    {
      for (int y = 0; y < Patterns[pattern].GetLength(1); ++y)
      {
        if (!component[x + deltaI, y + deltaJ] == Patterns[pattern][x, y]) return false;
      }
    }
    return CheckSides(pattern, component, deltaI, deltaJ);
  }

  private static bool CheckSides(Pattern pattern, bool[,] component, int deltaI, int deltaJ)
  {
    for (int x = deltaI - 1; x <= deltaI + Patterns[pattern].GetLength(0); ++x)
    {
      if (IsOnComponent(component, x, deltaJ - 1) && component[x, deltaJ - 1]) return false;
    }
    for (int x = deltaI - 1; x <= deltaI + Patterns[pattern].GetLength(0); ++x)
    {
      if (IsOnComponent(component, x, deltaJ + Patterns[pattern].GetLength(1)) &&
        component[x, deltaJ + Patterns[pattern].GetLength(1)]) return false;
    }

    for (int y = deltaJ - 1; y <= deltaJ + Patterns[pattern].GetLength(1); ++y)
    {
      if (IsOnComponent(component, deltaI - 1, y) && component[deltaI - 1, y]) return false;
    }
    for (int y = deltaJ - 1; y <= deltaJ + Patterns[pattern].GetLength(1); ++y)
    {
      if (IsOnComponent(component, deltaI + Patterns[pattern].GetLength(0), y) &&
        component[deltaI + Patterns[pattern].GetLength(0), y]) return false;
    }
    return true;
  }

  // private static bool MatchesBlock(bool[,] component, int i, int j)
  // {
  //   Console.WriteLine($"{i}, {j}");
  //   int deltaI = i - RootBlock[0];
  //   int deltaJ = j - RootBlock[1];
  //   if (deltaI < 0) return false;
  //   if (deltaJ < 0) return false;
  //   if (deltaI + PatternBlock.GetLength(0) > component.GetLength(0)) return false;
  //   if (deltaJ + PatternBlock.GetLength(1) > component.GetLength(1)) return false;

  //   for (int x = 0; x < PatternBlock.GetLength(0); ++x)
  //   {
  //     for (int y = 0; y < PatternBlock.GetLength(1); ++y)
  //     {
  //       if (!component[x + deltaI, y + deltaJ] == PatternBlock[x, y]) return false;
  //     }
  //   }
  //   return CheckSidesBlock(component, deltaI, deltaJ);
  // }

  // private static bool CheckSidesBlock(bool[,] component, int deltaI, int deltaJ)
  // {
  //   for (int x = deltaI - 1; x <= deltaI + PatternBlock.GetLength(0); ++x)
  //   {
  //     if (IsOnComponent(component, x, deltaJ - 1) && component[x, deltaJ - 1]) return false;
  //   }
  //   for (int x = deltaI - 1; x <= deltaI + PatternBlock.GetLength(0); ++x)
  //   {
  //     if (IsOnComponent(component, x, deltaJ + PatternBlock.GetLength(1)) &&
  //       component[x, deltaJ + PatternBlock.GetLength(1)]) return false;
  //   }

  //   for (int y = deltaJ - 1; y <= deltaJ + PatternBlock.GetLength(1); ++y)
  //   {
  //     if (IsOnComponent(component, deltaI - 1, y) && component[deltaI - 1, y]) return false;
  //   }
  //   for (int y = deltaJ - 1; y <= deltaJ + PatternBlock.GetLength(1); ++y)
  //   {
  //     if (IsOnComponent(component, deltaI + PatternBlock.GetLength(0), y) &&
  //       component[deltaI + PatternBlock.GetLength(0), y]) return false;
  //   }
  //   return true;
  // }

  private static bool IsOnComponent(bool[,] component, int i, int j)
  => i >= 0 && i < component.GetLength(0) && j >= 0 && j < component.GetLength(1);

}
