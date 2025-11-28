public class FavoritesForm : Form
{
  private Terrain _terrain;
  private int _cellSize = 10;
  private bool[,] _mask;

  public FavoritesForm(Terrain terrain)
  {
    _terrain = terrain;
    _mask = new bool[terrain.Width, terrain.Height];
    StartPosition = FormStartPosition.Manual;
    Location = new Point(620, 100);
    Size = new Size(600, 600);
    Text = "FavoritesForm";
    Paint += DrawTerrain;
  }

  public void ChangeMask(bool[,] mask)
  {
    if (mask.GetLength(0) != _terrain.Width || mask.GetLength(1) != _terrain.Height)
      throw new InvalidOperationException("Mask doesn't fit the terrain");
    _mask = mask;
  }

  private void DrawTerrain(object? sender, PaintEventArgs e)
  {
    e.Graphics.DrawImage(Render(), 0, 0);
  }

  private Bitmap Render()
  {
    Bitmap bmp = new Bitmap(_terrain.Width * _cellSize, _terrain.Height * _cellSize);
    for (int i = 0; i < _terrain.Height; ++i)
    {
      for (int j = 0; j < _terrain.Width; ++j)
      {
        DrawCell(i, j, Graphics.FromImage(bmp));
      }
    }
    return bmp;
  }
  
  private void DrawCell(int i, int j, Graphics g)
  {
    if (!_terrain.Field[i, j].IsAlive) return;
    Color cellColor = Color.Green;
    Brush brush = new SolidBrush(cellColor);
    g.FillRectangle(brush, i * _cellSize, j * _cellSize, _cellSize, _cellSize);
    brush.Dispose();

    Pen pen = new Pen(Color.Black);
    g.DrawRectangle(pen, i * _cellSize, j * _cellSize, _cellSize, _cellSize);
    pen.Dispose();
  }
}
