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
    foreach (Colony colony in Colonies!)
    {
      colony.Move();
    }
  }
}
