public interface IWorldFactory
{
  ICellStrategyProvider CreateStrategies();
  ICellFactory CreateCellFactory(ICellStrategyProvider strategies);
}
