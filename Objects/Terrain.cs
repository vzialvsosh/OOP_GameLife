public class Terrain
{
  // private Cell[,] _field;
  private double _possibitity = 0.3;

  public int Width { get; private set; }
  public int Height { get; private set; }
  public Cell[,] Field { get; private set; }
  public HashSet<Colony> Colonies { get; private set; } = new();
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
        // if (Field[i, j].IsAlive) terrain.Field[i, j] = new Cell(true, terrain, i, j);
        // else terrain.Field[i, j] = new Cell(false, terrain, i, j);
        terrain.Field[i, j] = Field[i, j].GetCopy(terrain);
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
        updatedField[i, j] = Field[i, j].GenerateNextCell();
      }
    }
    Field = updatedField;
    foreach (Colony colony in Colonies)
    {
      colony.UpdateMembers();
    }
    foreach (Colony colony in Colonies)
    {
      colony.Move();
    }
  }

  public bool IsOnField(int i, int j) => i >= 0 && i < Width && j >= 0 && j < Height;
  
  public void OrganizeColony(object? sender, StablePatternEvetnArgs args)
  {
    Colony colony = new(this);
    for (int i = args.Frame.minX; i <= args.Frame.maxX; ++i)
    {
      for (int j = args.Frame.minY; j <= args.Frame.maxY; ++j)
      {
        if (!args.Component[i, j]) continue;
        colony.AddNextMember(Field[i, j]);
        Field[i, j].SetColony(colony);
      }
    }
    Colonies.Add(colony);
  }

  public void RemoveColony(Colony colony)
  {
    Colonies.Remove(colony);
  }
}
