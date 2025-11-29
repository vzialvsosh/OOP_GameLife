public class LifeFrom : Form
{
  private Terrain _terrain;
  private int _cellSize = 10;

  public LifeFrom(Terrain terrain)
  {
    _terrain = terrain;
    StartPosition = FormStartPosition.Manual;
    Location = new Point(20, 100);
    Size = new Size(600, 600);
    Text = "LifeForm";
    Paint += DrawTerrain;
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
    Color cellColor = _terrain.Field[i, j].IsWhite() ? Color.Green : Color.Red;
    Brush brush = new SolidBrush(cellColor);
    g.FillRectangle(brush, i * _cellSize, j * _cellSize, _cellSize, _cellSize);
    brush.Dispose();

    Pen pen = new Pen(Color.Black);
    g.DrawRectangle(pen, i * _cellSize, j * _cellSize, _cellSize, _cellSize);
    pen.Dispose();
  }
}
