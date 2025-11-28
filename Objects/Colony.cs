public class Colony
{
  public int Count { get; private set; }
  public HashSet<Cell> Members { get; private set; } = new();
}
