public class Cell
{
  public bool Alive { get; private set; }

  public Cell(bool alive)
  {
    Alive = alive;
  }

  public void MakeAlive()
  {
    Alive = true;
  }

  public void Kill()
  {
    Alive = false;
  }
}
