public class Terrain
{
  // private Cell[,] _field;
  private double _possibitity = 0.3;

  public int Width { get; private set; }
  public int Height { get; private set; }
  public Cell[,] Field { get; private set; }
  // public Cell[,] Field
  // {
  //   get => _field;
  //   private set => _field = value;
  // }
  
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
        if (Field[i, j].IsAlive) terrain.Field[i, j] = new Cell(true, terrain, i, j);
        else terrain.Field[i, j] = new Cell(false, terrain, i, j);
      }
    }
    return terrain;
  }

  public void CopyTo(Terrain terrain)
  {
    if (terrain.Width != Width || terrain.Height != Height) throw new InvalidOperationException("Width or heigth of terrain to copy doesn't fit the origin ones");
    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        if (Field[i, j].IsAlive) terrain.Field[i, j] = new Cell(true, terrain, i, j);
        else terrain.Field[i, j] = new Cell(false, terrain, i, j);
      }
    }
  }

  private void FillField()
  {
    Random rnd = new Random();
    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        Field[i, j] = new Cell(rnd.NextDouble() < _possibitity, this, i, j);
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
        Field[i, j] = new Cell(array[i, j], this, i, j);
      }
    }
  }

  public void Clear()
  {
    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        Field[i, j] = new Cell(false, this, i, j);
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
        updatedField[i, j] = new Cell(Field[i, j].WillBeAlive(), this, i, j);
      }
    }
    Field = updatedField;
  }

  public bool IsOnField(int i, int j) => i >= 0 && i < Width && j >= 0 && j < Height;
  
}
