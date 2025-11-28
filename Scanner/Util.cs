public static class Util
{
  public static bool ArraysEqual(bool[,] array1, bool[,] array2)
  {
    if (array1.GetLength(0) != array2.GetLength(0)) return false;
    if (array1.GetLength(1) != array2.GetLength(1)) return false;

    for (int i = 0; i < array1.GetLength(0); ++i)
    {
      for (int j = 0; j < array1.GetLength(1); ++j)
      {
        if (array1[i, j] != array2[i, j]) return false;
      }
    }
    return true;
  }

  public static bool[,] GetNextPhase(bool[,] array)
  {
    bool[,] newArray = new bool[array.GetLength(0), array.GetLength(1)];
    for (int i = 0; i < newArray.GetLength(0); ++i)
    {
      for (int j = 0; j < newArray.GetLength(1); ++j)
      {
        int cnt = GetCountNeighbours(array, i, j);
        if (array[i, j] && (cnt < 2 || cnt > 3)) newArray[i, j] = false;
        else if (!array[i, j] && cnt == 3) newArray[i, j] = true;
        else newArray[i, j] = array[i, j];
      }
    }
    return newArray;
  }

  public static int GetCountNeighbours(bool[,] array, int i, int j)
  {
    int cnt = array[i, j] ? -1 : 0;
    for (int x = i - 1; x <= i + 1; ++x)
    {
      for (int y = j - 1; y <= j + 1; ++y)
      {
        if (IsOnArray(array, x, y) && array[x, y]) ++cnt;
      }
    }
    return cnt;
  }

  public static bool IsOnArray(bool[,] array, int i, int j) =>
    i >= 0 && i < array.GetLength(0) && j >= 0 && j < array.GetLength(1);
}
