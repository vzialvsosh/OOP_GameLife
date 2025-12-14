public class ClassicWorldFactory : IWorldFactory
{
    public ICellStrategyProvider CreateStrategies()
    {
        var map = new Dictionary<CellType, ICellLifeStrategy>
        {
            { CellType.Empty, new ClassicEmptyStrategy() },
            { CellType.White, new ClassicWhiteStrategy() },
            { CellType.Black, new ClassicWhiteStrategy() }
        };

        return new CellStrategyProvider(map);
    }

    public ICellFactory CreateCellFactory(ICellStrategyProvider strategies)
    {
        return new SimpleCellFactory(strategies);
    }
}
