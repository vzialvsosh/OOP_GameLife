public class Terrain : Form
{
  private double _possibitity = 0.3;

  public IWorldFactory WorldFactory;
  protected ICellFactory _cellFactory;

  public virtual string Mode { get; private set; }

  public virtual int TerrainWidth { get; private set; }
  public virtual int TerrainHeight { get; private set; }
  public virtual Cell[,] Field { get; private set; }
  public virtual HashSet<Colony>? Colonies { get; private set; }

  protected int _cellSize = 10;
  private bool _visibleNet = false;
  public virtual bool OnPause { get; private set; } = false;

  public void SetName(string name)
  {
    Text = name;
  }

  public void InitializeColonies()
  {
    Colonies = new();
  }

  public void SetLocation(int locationX, int locationY)
  {
    StartPosition = FormStartPosition.Manual;
    Location = new Point(locationX, locationY);
  }

  public void SetSize(int sizeX, int sizeY)
  {
    Size = new Size(sizeX, sizeY);
  }

  public void SetDefaultSize()
  {
    Size = new Size(_cellSize * TerrainWidth + 250, Math.Max(_cellSize * TerrainHeight + 100, 400));
  }

  public Terrain(int terrainWidth, int terrainHeight, string mode)
  {
    TerrainWidth = terrainWidth;
    TerrainHeight = terrainHeight;
    Field = new Cell[TerrainWidth, TerrainHeight];
    FillField();

    // OnModeChanged("Classic");
    Mode = mode;
    OnModeChanged(mode);

    // ClearMask();
    Paint += Draw;
  }

  private void OnModeChanged(string mode)
  {
    WorldFactory = mode switch
    {
      "Classic" => new ClassicWorldFactory(),
      "Colonies" => new ColoniesWorldFactory(),
      _ => throw new NotImplementedException()
    };

    var strategies = WorldFactory.CreateStrategies();
    var cellFactory = WorldFactory.CreateCellFactory(strategies);
    Reinitialize(cellFactory);
  }

  public void Reinitialize(ICellFactory cellFactory)
  {
    _cellFactory = cellFactory;
  }

  public virtual Terrain GetCopy()
  {
    Terrain terrain = new Terrain(TerrainWidth, TerrainHeight, "Classic");
    for (int i = 0; i < TerrainWidth; ++i)
    {
      for (int j = 0; j < TerrainHeight; ++j)
      {
        terrain.Field[i, j] = Field[i, j].GetCopy(terrain);
      }
    }
    return terrain;
  }

  public virtual void CopyTo(Terrain terrain)
  {
    if (terrain.TerrainWidth != TerrainWidth || terrain.TerrainHeight != TerrainHeight) throw new InvalidOperationException("TerrainWidth or heigth of terrain to copy doesn't fit the origin ones");
    for (int i = 0; i < TerrainWidth; ++i)
    {
      for (int j = 0; j < TerrainHeight; ++j)
      {
        if (Field[i, j].IsAlive) terrain.Field[i, j] = new Cell(true, terrain, i, j);
        else terrain.Field[i, j] = new Cell(false, terrain, i, j);
      }
    }
  }

  private void FillField()
  {
    Random rnd = new Random();
    for (int i = 0; i < TerrainWidth; ++i)
    {
      for (int j = 0; j < TerrainHeight; ++j)
      {
        Field[i, j] = new Cell(rnd.NextDouble() < _possibitity, this, i, j);
      }
    }
  }

  // public void FillByArray(bool[,] array)
  // {
  //   if (array.GetLength(0) != TerrainWidth) throw new Exception("Array does not fit the terrain");
  //   if (array.GetLength(1) != TerrainHeight) throw new Exception("Array does not fit the terrain");
  //   for (int i = 0; i < TerrainWidth; ++i)
  //   {
  //     for (int j = 0; j < TerrainHeight; ++j)
  //     {
  //       Field[i, j] = new Cell(array[i, j], this, i, j);
  //     }
  //   }
  // }

  public virtual void Clear()
  {
    for (int i = 0; i < TerrainWidth; ++i)
    {
      for (int j = 0; j < TerrainHeight; ++j)
      {
        Field[i, j] = new Cell(false, this, i, j);
      }
    }
  }

  public virtual void UpdateField()
  {
    Cell[,] updatedField = new Cell[TerrainWidth, TerrainHeight];
    for (int i = 0; i < TerrainWidth; ++i)
    {
      for (int j = 0; j < TerrainHeight; ++j)
      {
        // updatedField[i, j] = Field[i, j].GenerateNextCell();
        updatedField[i, j] = _cellFactory.Create(i, j, Field[i, j].GetCellType(), this);
      }
    }
    Field = updatedField;
    // foreach (Colony colony in Colonies)
    // {
    //   colony.UpdateMembers();
    // }
    // foreach (Colony colony in Colonies)
    // {
    //   colony.Move();
    // }
  }

  public virtual bool IsOnField(int i, int j) => i >= 0 && i < TerrainWidth && j >= 0 && j < TerrainHeight;

  public virtual void OrganizeColony(object? sender, StablePatternEvetnArgs args)
  {
    if (Colonies == null) return;
    Colony colony = new(this);
    for (int i = args.Frame.minX; i <= args.Frame.maxX; ++i)
    {
      for (int j = args.Frame.minY; j <= args.Frame.maxY; ++j)
      {
        if (!args.Component[i, j]) continue;
        Field[i, j].SetColony(colony);
      }
    }
    colony.UpdateMembers();
    Colonies.Add(colony);
  }

  public virtual void RemoveColony(Colony colony)
  {
    Colonies?.Remove(colony);
  }

  protected void Draw(object? sender, PaintEventArgs e)
  {
    // UpdateField();
    DrawTerrain(e, null, null);
    // DrawFeatures(Controls);
  }

  public virtual void DrawTerrain(PaintEventArgs e, bool[,]? mask, bool? ignoreMask)
  {
    e.Graphics.DrawImage(Render(mask, ignoreMask), 0, 0);
  }

  public virtual Bitmap Render(bool[,]? mask, bool? ignoreMask)
  {
    if (mask != null && (mask.GetLength(0) != TerrainWidth || mask.GetLength(1) != TerrainHeight)) throw new InvalidOperationException("Mask is not suitable");
    Bitmap bmp = new Bitmap(TerrainWidth * _cellSize, TerrainHeight * _cellSize);
    var g = Graphics.FromImage(bmp);
    g.Clear(Color.White);
    if (_visibleNet)
    {
      Pen pen = new(Color.Gray);
      for (int i = 0; i < bmp.Width; i += _cellSize)
      {
        g.DrawLine(pen, i, 0, i, bmp.Height);
      }
      for (int j = 0; j < bmp.Height; j += _cellSize)
      {
        g.DrawLine(pen, 0, j, bmp.Width, j);
      }
    }
    for (int i = 0; i < TerrainWidth; ++i)
    {
      for (int j = 0; j < TerrainHeight; ++j)
      {
        DrawCell(i, j, mask, ignoreMask, g);
      }
    }
    return bmp;
  }

  private void DrawCell(int i, int j, bool[,]? mask, bool? ignoreMask, Graphics g)
  {
    if (mask != null && ignoreMask != null &&
      ((!(bool)mask[i, j] && !(bool)ignoreMask))) return;
    if (!Field[i, j].IsAlive) return;
    Color cellColor = Field[i, j].IsWhite() ? Color.Green : Color.Red;
    Brush brush = new SolidBrush(cellColor);
    g.FillRectangle(brush, i * _cellSize, j * _cellSize, _cellSize, _cellSize);
    brush.Dispose();

    Pen pen = new Pen(Color.Black);
    g.DrawRectangle(pen, i * _cellSize, j * _cellSize, _cellSize, _cellSize);
    pen.Dispose();
  }

  public virtual void DrawFeatures(Control.ControlCollection controls)
  {
    CheckBox checkBoxVisibleNet = new();
    checkBoxVisibleNet.Text = "Visible net";
    checkBoxVisibleNet.Size = new Size(100, 20);
    checkBoxVisibleNet.Location = new Point(_cellSize * TerrainWidth + 10, 10);
    checkBoxVisibleNet.CheckedChanged += (sender, e) => { _visibleNet = checkBoxVisibleNet.Checked; };

    CheckBox checkBoxOnPause = new();
    checkBoxOnPause.Text = "OnPause:";
    checkBoxOnPause.Size = new Size(100, 20);
    checkBoxOnPause.Location = new Point(_cellSize * TerrainWidth + 110, 10);
    checkBoxOnPause.CheckedChanged += (sender, e) => { OnPause = checkBoxOnPause.Checked; };

    Button buttonExit = new();
    buttonExit.Text = "Exit";
    buttonExit.Size = new Size(50, 30);
    buttonExit.Location = new Point(_cellSize * TerrainWidth + 10, 350);
    buttonExit.Click += (sender, e) => Application.Exit();

    controls.Add(checkBoxVisibleNet);
    controls.Add(checkBoxOnPause);
    controls.Add(buttonExit);
  }
}
