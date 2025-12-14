public class ColoniesWorldFactory : IWorldFactory
{
    public ICellStrategyProvider CreateStrategies()
    {
        var map = new Dictionary<CellType, ICellLifeStrategy>
        {
            { CellType.Empty, new ClassicEmptyStrategy() },
            { CellType.White, new ColonyWhiteStrategy() },
            { CellType.Black, new ColonyBlackStrategy() }
        };

        return new CellStrategyProvider(map);
    }

    public ICellFactory CreateCellFactory(ICellStrategyProvider strategies)
    {
        return new SimpleCellFactory(strategies);
    }
}
