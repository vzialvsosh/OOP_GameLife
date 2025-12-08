using System.Diagnostics;

public class StatisticsTerrainDecorator : TerrainDecorator
{
  public StatisticsTerrainDecorator(Terrain terrain) : base(terrain) { }

  private int _cntWhite = 0;
  private int _cntBlack = 0;
  private int _lastCntWhite = 1;
  private int _lastCntBlack = 1;
  private double _updateDuration = 0;
  private double _scanDuration = 0;

  private event Action? countsUpdated;
  private event Action? updated;
  private event Action? scanned;

  public override void DrawTerrain(PaintEventArgs e, bool[,]? mask, bool? ignoreMask)
  {
    if (_terrain is ScannerTerrainDecorator terrain)
    {
      var stopwatch = Stopwatch.StartNew();
      terrain.Scan();
      stopwatch.Stop();

      terrain.DrawTerrainWithoutScan(e);

      _scanDuration = stopwatch.Elapsed.TotalSeconds;
      scanned?.Invoke();
    }
    else base.DrawTerrain(e, mask, ignoreMask);
  }

  public override void UpdateField()
  {
    var stopwatch = Stopwatch.StartNew();
    base.UpdateField();
    stopwatch.Stop();

    _updateDuration = stopwatch.Elapsed.TotalSeconds;
    updated?.Invoke();

    UpdateCounters();
  }

  public override void DrawFeatures(Control.ControlCollection controls)
  {
    _terrain.DrawFeatures(controls);

    UpdateCounters();
    AddCounters(controls);
    AddDurations(controls);
  }

  private void AddCounters(Control.ControlCollection controls)
  {
    Label labelAlive = new();
    labelAlive.Text = $"Number of cells alive: {_cntWhite + _cntBlack}";
    if (_lastCntBlack + _lastCntWhite != 0)
      labelAlive.Text += $"( {100 * (double)(_cntWhite - _lastCntWhite + _cntBlack - _lastCntBlack) / (_lastCntBlack + _lastCntWhite):+0.###;-0.###}% )";

    labelAlive.Size = new Size(300, 20);
    labelAlive.Location = new Point(510, 100);

    Label labelWhite = new();
    labelWhite.Text = $"Number of white cells: {_cntWhite}";
    if (_lastCntWhite != 0) labelWhite.Text += $"( {100 * (double)(_cntWhite - _lastCntWhite) / _lastCntWhite:+0.###;-0.###}% )";

    labelWhite.Size = new Size(300, 20);
    labelWhite.Location = new Point(510, 130);

    Label labelBlack = new();
    labelBlack.Text = $"Number of black cells: {_cntBlack}";
    if (_lastCntBlack != 0) labelBlack.Text += $"( {100 * (double)(_cntBlack - _lastCntBlack) / _lastCntBlack:+0.###;-0.###}% )";

    labelBlack.Size = new Size(300, 20);
    labelBlack.Location = new Point(510, 160);

    controls.Add(labelAlive);
    controls.Add(labelWhite);
    controls.Add(labelBlack);

    countsUpdated += () =>
    {
      labelAlive.Text = $"Number of cells alive: {_cntWhite + _cntBlack}";
      if (_lastCntBlack + _lastCntWhite != 0)
        labelAlive.Text += $"( {100 * (double)(_cntWhite - _lastCntWhite + _cntBlack - _lastCntBlack) / (_lastCntBlack + _lastCntWhite):+0.###;-0.###}% )";

      labelWhite.Text = $"Number of white cells: {_cntWhite}";
      if (_lastCntWhite != 0) labelWhite.Text += $"( {100 * (double)(_cntWhite - _lastCntWhite) / _lastCntWhite:+0.###;-0.###}% )";

      labelBlack.Text = $"Number of black cells: {_cntBlack}";
      if (_lastCntBlack != 0) labelBlack.Text += $"( {100 * (double)(_cntBlack - _lastCntBlack) / _lastCntBlack:+0.###;-0.###}% )";
    };
  }

  private void AddDurations(Control.ControlCollection controls)
  {
    Label labelUpdateDuration = new();
    labelUpdateDuration.Text = $"Updating duration: {_updateDuration:F4} sec";
    labelUpdateDuration.Size = new Size(200, 20);
    labelUpdateDuration.Location = new Point(510, 200);

    controls.Add(labelUpdateDuration);
    updated += () => labelUpdateDuration.Text = $"Updating duration: {_updateDuration:F4} sec";

    Label labelScanDuration = new();
    labelScanDuration.Text = $"Scanning duration: {_scanDuration:F4} sec";
    labelScanDuration.Size = new Size(200, 50);
    labelScanDuration.Location = new Point(510, 230);

    controls.Add(labelScanDuration);
    scanned += () => labelScanDuration.Text = $"Scanning duration: {_scanDuration:F4} sec";
  }

  // public override void UpdateField()
  // {
  //   base.UpdateField();
  //   UpdateCounters();
  // }

  private void UpdateCounters()
  {
    _lastCntWhite = _cntWhite;
    _lastCntBlack = _cntBlack;
    _cntWhite = 0;
    _cntBlack = 0;
    for (int i = 0; i < TerrainWidth; ++i)
    {
      for (int j = 0; j < TerrainHeight; ++j)
      {
        if (Field[i, j].IsWhite()) ++_cntWhite;
        else if (Field[i, j].IsBlack()) ++_cntBlack;
      }
    }
    countsUpdated?.Invoke();
  }
}
