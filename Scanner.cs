public static class Scanner {
  private static bool[,] PatterBlock = {
    { true, true},
    { true, true}
  };
  private static int[] RootBlock = { 0, 0};
 
  public static Terrain Scan(Terrain terrain)
  {
    Terrain newTerrain = terrain.GetCopy();
    newTerrain.FillByArray(GetComponents(newTerrain));
    return newTerrain;
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
    return MatchesBlock(component, i, j);
  }

  private static bool MatchesBlock(bool[,] component, int i, int j)
  {
    int deltaI = i - RootBlock[0];
    int deltaJ = j - RootBlock[1];
    if (deltaI < 0) return false;
    if (deltaJ < 0) return false;
    if (deltaI + PatterBlock.GetLength(0) > component.GetLength(0)) return false;
    if (deltaJ + PatterBlock.GetLength(1) > component.GetLength(1)) return false;

    for (int x = 0; x < PatterBlock.GetLength(0); ++x)
    {
      for (int y = 0; y < PatterBlock.GetLength(1); ++y)
      {
        if (!component[x + deltaI, y + deltaJ] == PatterBlock[x, y]) return false;
      }
    }
    return true;
  }
}
