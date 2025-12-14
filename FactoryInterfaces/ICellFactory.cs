public interface ICellFactory
{
  Cell Create(int x, int y, CellType cellType, Terrain terrain);
}
