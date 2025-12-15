public class SimpleCellFactory : ICellFactory
{
  private readonly ICellStrategyProvider strategies;

  public SimpleCellFactory(ICellStrategyProvider strategies)
  {
    this.strategies = strategies;
  }

  public Cell Create(int x, int y, CellType type, Terrain terrain)
  {
    // return new Cell(x, y, type, strategies);
    Cell cell = new Cell(strategies, terrain, x, y);
    return cell;
  }
}
