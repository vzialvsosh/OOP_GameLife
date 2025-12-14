public class ClassicEmptyStrategy : ICellLifeStrategy
{
    public CellType GetNext(CellContext ctx)
    {
        // return ctx.cntAlive == 3 ? CellType.White : CellType.Empty;
        return ctx.cntWhite == 3 ? CellType.White : CellType.Empty;
    }
}

public class ClassicWhiteStrategy : ICellLifeStrategy
{
    public CellType GetNext(CellContext ctx)
    {
        return ctx.cntAlive is 2 or 3 ? CellType.White : CellType.Empty;
    }
}
