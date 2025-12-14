public class CellStrategyProvider : ICellStrategyProvider
{
    private readonly Dictionary<CellType, ICellLifeStrategy> strategies;

    public CellStrategyProvider(Dictionary<CellType, ICellLifeStrategy> strategies)
    {
        this.strategies = strategies;
    }

    public ICellLifeStrategy For(CellType type)
    {
        return strategies[type];
    }
}
