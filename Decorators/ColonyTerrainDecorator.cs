public class ColonyTerrainDecorator : TerrainDecorator
{
  public ColonyTerrainDecorator(Terrain terrain) : base(terrain)
  {
    terrain.InitializeColonies();
  }

  public override void UpdateField()
  {
    base.UpdateField();
    foreach (Colony colony in Colonies!)
    {
      colony.UpdateMembers();
    }
    if (Mode != "Colonies") return;
    foreach (Colony colony in Colonies!)
    {
      colony.Move();
    }
  }
}
