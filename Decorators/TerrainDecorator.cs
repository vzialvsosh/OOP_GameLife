
abstract public class TerrainDecorator : Terrain
{
  protected Terrain _terrain;

  private int fective = 0;
  public override string Mode => _terrain.Mode;

  public override int TerrainWidth => _terrain?.TerrainWidth ?? fective;
  public override int TerrainHeight => _terrain?.TerrainHeight ?? fective;
  public override Cell[,] Field => _terrain.Field;
  public override HashSet<Colony>? Colonies => _terrain.Colonies;

  // public override bool IgnoreMask => _terrain.IgnoreMask;
  public override bool OnPause => _terrain.OnPause;

  public TerrainDecorator(Terrain terrain) : base(0, 0, "Classic")
  {
    _terrain = terrain;
  }

  public override void OnModeChanged(string mode) => _terrain?.OnModeChanged(mode);

  public override Terrain GetCopy() => _terrain.GetCopy();

  public override void CopyTo(Terrain terrain) => _terrain.CopyTo(terrain);

  public override void Clear() => _terrain.Clear();

  public override bool IsOnField(int i, int j) => _terrain.IsOnField(i, j);

  public override void OrganizeColony(object? sender, StablePatternEvetnArgs args) =>
    _terrain.OrganizeColony(sender, args);

  public override void RemoveColony(Colony colony) => _terrain.RemoveColony(colony);

  // public override void ChangeMask(bool[,] mask) => _terrain.ChangeMask(mask);

  // public override void SetIgnoreMask(bool ignoreMask) => _terrain.SetIgnoreMask(ignoreMask);

  // public override void ClearMask() => _terrain?.ClearMask();

  public override void UpdateField() => _terrain.UpdateField();

  // protected override void Draw(object? sender, PaintEventArgs e)
  // {
  //   throw new NotImplementedException("'Draw' should me implemented");
  // }

  public override void DrawTerrain(PaintEventArgs e, bool[,]? mask, bool? ignoreMask) =>
    _terrain.DrawTerrain(e, mask, ignoreMask);

  public override void DrawFeatures(Control.ControlCollection controls) =>
    _terrain.DrawFeatures(controls);

  public override Bitmap Render(bool[,]? mask, bool? ignoreMask) => _terrain.Render(mask, ignoreMask);
}
