public class Terrain
{
  private Cell[,] _field;
  private double _possibitity = 0.4;

  public int Width { get; private set; }
  public int Height { get; private set; }
  public Cell[,] Field
  {
    get => _field;
    private set => _field = value;
  }

  public Terrain(int width, int height)
  {
    Width = width;
    Height = height;
    Field = new Cell[width, height];
    FillField();
  }

  public Terrain GetCopy()
  {
    Terrain terrain = new Terrain(Width, Height);
    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        if (Field[i, j].Alive) terrain.Field[i, j] = new Cell(true);
        else terrain.Field[i, j] = new Cell(false);
      }
    }
    return terrain;
  }

  private void FillField()
  {
    Random rnd = new Random();
    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        Field[i, j] = new Cell(rnd.NextDouble() < _possibitity);
      }
    }
    for (int i = 0; i < 2; ++i)
    {
      for (int j = 0; j < 2; ++j)
      {
        Field[i, j] = new Cell(true);
      }
    }
  }

  public void FillByArray(bool[,] array)
  {
    if (array.GetLength(0) != Width) throw new Exception("Array does not fit the terrain");
    if (array.GetLength(1) != Height) throw new Exception("Array does not fit the terrain");
    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        Field[i, j] = new Cell(array[i, j]);
      }
    }
  }

  public void Clear()
  {
    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        Field[i, j] = new Cell(false);
      }
    }
  }

  public void UpdateField()
  {
    Cell[,] updatedField = new Cell[Width, Height];
    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        updatedField[i, j] = UpdatedCell(i, j);
      }
    }
    Field = updatedField;
  }

  private Cell UpdatedCell(int i, int j)
  {
    int cnt = CellNeighboursCount(i, j);
    if (Field[i, j].Alive && (cnt < 2 || cnt > 3)) return new Cell(false);
    else if (!Field[i, j].Alive && cnt == 3) return new Cell(true);
    return new Cell(Field[i, j].Alive);
  }

  private int CellNeighboursCount(int i, int j)
  {
    int cnt = Field[i, j].Alive ? -1 : 0;
    for (int x = i - 1; x <= i + 1; ++x)
    {
      for (int y = j - 1; y <= j + 1; ++y)
      {
        if (IsOnField(x, y) && Field[x, y].Alive) ++cnt;
      }
    }
    return cnt;
  }

  public bool IsOnField(int i, int j)
  {
    return i >= 0 && i < Width && j >= 0 && j < Height;
  }
}
