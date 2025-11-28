public class Patterns
{
  public struct Pattern
  {
    public bool[,] array;
    public int[] root;
  }
  private enum InitialPatternNames
  {
    Block,
    Blinker,
    BeeHive,
    PentaDecathlon
  }
  private static Dictionary<InitialPatternNames, Pattern> InitailPatterns = new Dictionary<InitialPatternNames, Pattern>
  {
    [InitialPatternNames.Block] = new Pattern
    {
      array = new bool[,] {
      { true, true },
      { true, true }
    },
      root = new int[] { 0, 0 }
    },
    [InitialPatternNames.Blinker] = new Pattern
    {
      array = new bool[,]
    {
      { false, false, false },
      { true, true, true },
      { false, false, false }
    },
      root = new int[] { 0, 0 }
    },
    [InitialPatternNames.BeeHive] = new Pattern
    {
      array = new bool[,]
    {
      { false, true, false },
      { true, false, true },
      { true, false, true },
      { false, true, false }
    },
      root = new int[] { 0, 1 }
    },
    [InitialPatternNames.PentaDecathlon] = new Pattern
    {
      array = new bool[,]
    {
      { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
      { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
      { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },

      { false, false, false, false, false, true,  false, false, false, false, true,  false, false, false, false, false },
      { false, false, false, true,  true,  false, true,  true,  true,  true,  false, true,  true,  false, false, false },
      { false, false, false, false, false, true,  false, false, false, false, true,  false, false, false, false, false },

      { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
      { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
      { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false }
    }, root = new int[] { 3, 5 }
    }
  };

  public HashSet<Pattern> AllPatterns { get; private set; }

  public Patterns()
  {
    AllPatterns = GetAllPatterns();
  }

  private HashSet<Pattern> GetAllPatterns()
  {
    HashSet<Pattern> allPhases = new();
    foreach (InitialPatternNames name in InitailPatterns.Keys)
    {
      allPhases.UnionWith(GetAllPhases(InitailPatterns[name]));
    }

    HashSet<Pattern> patterns = new();
    foreach (Pattern pattern in allPhases)
    {
      patterns.UnionWith(GetAllRotations(pattern));
    }
    return patterns;
  }

  private HashSet<Pattern> GetAllRotations(Pattern pattern)
  {
    HashSet<Pattern> rotations = new HashSet<Pattern> { pattern };
    for (int i = 0; i < 3; ++i)
    {
      pattern = RotateRight(pattern);
      rotations.Add(pattern);
    }
    return rotations;
  }

  private HashSet<Pattern> GetAllPhases(Pattern pattern)
  {
    HashSet<Pattern> phases = new HashSet<Pattern> { pattern };
    Pattern nextPhase = GetNextPhase(pattern);
    while (!Util.ArraysEqual(nextPhase.array, pattern.array))
    {
      phases.Add(nextPhase);
      nextPhase = GetNextPhase(nextPhase);
    }
    return phases;
  }

  private Pattern GetNextPhase(Pattern pattern)
  {
    Pattern nextPhase = new Pattern { array = Util.GetNextPhase(pattern.array) };
    nextPhase.root = GetRoot(nextPhase);
    return nextPhase;
  }

  private Pattern RotateRight(Pattern pattern)
  {
    Pattern rotatedPattern = new();
    rotatedPattern.array = new bool[pattern.array.GetLength(1), pattern.array.GetLength(0)];
    for (int i = 0; i < pattern.array.GetLength(0); ++i)
    {
      for (int j = 0; j < pattern.array.GetLength(1); ++j)
      {
        rotatedPattern.array[j, pattern.array.GetLength(0) - 1 - i] = pattern.array[i, j];
      }
    }
    rotatedPattern.root = GetRoot(rotatedPattern);
    return rotatedPattern;
  }

  private int[] GetRoot(Pattern pattern)
  {
    for (int i = 0; i < pattern.array.GetLength(0); ++i)
    {
      for (int j = 0; j < pattern.array.GetLength(1); ++j)
      {
        if (pattern.array[i, j] == true)
        {
          return new int[] { i, j };
        }
      }
    }
    throw new Exception("The pattern is empty");
  }
}