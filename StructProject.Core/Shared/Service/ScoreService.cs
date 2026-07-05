namespace StructProject.Core.Shared.Service;

public class ScoreService : IScoreService
{
  private int _score;

  public int CurrentScore => _score;
  public event System.Action<int>? OnScoreChanged;

  public void AddScore(int amount)
  {
    if (amount <= 0) return;
    _score += amount;
    OnScoreChanged?.Invoke(_score);
  }
}
