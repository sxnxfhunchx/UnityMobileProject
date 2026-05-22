namespace Interfaces
{
    public interface ILevelProvider
    {
        LevelSettings CurrentLevelSettings { get; }
        bool IsRegularEnemyPhaseActive { get; }
        bool IsBossPhaseActive { get; }
    }
}