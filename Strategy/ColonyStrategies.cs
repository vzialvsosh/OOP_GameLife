public class ColonyWhiteStrategy : ICellLifeStrategy
{
    public CellType GetNext(CellContext ctx)
    {
        // if (ctx.cntBlack > ctx.cntWhite) return CellType.Black;
        // return ctx.cntAlive is 2 or 3 ? CellType.White : CellType.Empty;
        if (ctx.cntBlack > ctx.cntWhite + 1) return CellType.Black;
        return ctx.cntWhite is 2 or 3 ? CellType.White : CellType.Empty;
    }
}

public class ColonyBlackStrategy : ICellLifeStrategy
{
    public CellType GetNext(CellContext ctx)
    {
        // if (ctx.cntWhite > ctx.cntBlack) return CellType.White;
        // return ctx.cntAlive is 1 or 2 or 3 ? CellType.Black : CellType.Empty;
        if (ctx.cntWhite > ctx.cntBlack + 1) return CellType.White;
        return ctx.cntBlack is 2 or 3 ? CellType.Black : CellType.Empty;
    }
}
