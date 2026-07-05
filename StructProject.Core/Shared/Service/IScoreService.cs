namespace StructProject.Core.Shared.Service;

public interface IScoreService
{
  void AddScore(int amount);
  int CurrentScore { get; }
  event System.Action<int>? OnScoreChanged;
}
