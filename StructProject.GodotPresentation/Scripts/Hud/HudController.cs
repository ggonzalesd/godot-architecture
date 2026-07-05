using Godot;
using StructProject.Core.Entities.Skills;
using StructProject.GodotPresentation.Scripts.Containers;

namespace StructProject.GodotPresentation.Scripts.Hud;

public partial class HudController : CanvasLayer
{
  [Export] private NodePath HealthBarPath { get; set; } = "TopBar/HealthBox/HealthBar";
  [Export] private NodePath HealthLabelPath { get; set; } = "TopBar/HealthBox/HealthLabel";
  [Export] private NodePath ScoreLabelPath { get; set; } = "TopBar/ScoreLabel";
  [Export] private NodePath CoinsLabelPath { get; set; } = "TopBar/CoinsLabel";
  [Export] private NodePath WaveLabelPath { get; set; } = "TopBar/WaveLabel";
  [Export] private NodePath SkillBarPath { get; set; } = "SkillBar";

  private ProgressBar _healthBar = null!;
  private Label _healthLabel = null!;
  private Label _scoreLabel = null!;
  private Label _coinsLabel = null!;
  private Label _waveLabel = null!;
  private HBoxContainer _skillBar = null!;

  public override void _Ready()
  {
    _healthBar = GetNode<ProgressBar>(HealthBarPath);
    _healthLabel = GetNode<Label>(HealthLabelPath);
    _scoreLabel = GetNode<Label>(ScoreLabelPath);
    _coinsLabel = GetNode<Label>(CoinsLabelPath);
    _waveLabel = GetNode<Label>(WaveLabelPath);
    _skillBar = GetNode<HBoxContainer>(SkillBarPath);

    CallDeferred(MethodName.Initialize);
  }

  private void Initialize()
  {
    var c = BaseContainer.Instance;
    c.OnPlayerHealthChanged += hp => UpdateHealth(hp, c.Player.MaxHealth);
    c.OnPlayerScoreChanged += s => _scoreLabel.Text = $"Score: {s}";
    c.OnCoinsChanged += cn => _coinsLabel.Text = $"Coins: {cn}";
    c.OnWaveIndexChanged += w => _waveLabel.Text = $"Wave {w}";

    UpdateHealth(c.Player.CurrentHealth, c.Player.MaxHealth);
    _scoreLabel.Text = "Score: 0";
    _coinsLabel.Text = "Coins: 0";

    BuildSkillButtons();
  }

  private void UpdateHealth(int hp, int max)
  {
    _healthLabel.Text = $"HP {hp}/{max}";
    _healthBar.MaxValue = max;
    _healthBar.Value = hp;
  }

  private void BuildSkillButtons()
  {
    if (_skillBar == null) return;
    foreach (var child in _skillBar.GetChildren())
    {
      child.QueueFree();
    }
    var c = BaseContainer.Instance;
    foreach (var (kind, _) in c.SkillsState.Slots)
    {
      if (c.SkillRegistry.Get(kind) is not { } cfg) continue;

      var panel = new PanelContainer();
      var vBox = new VBoxContainer();
      panel.AddChild(vBox);

      var label = new Label
      {
        Text = cfg.DisplayName,
        HorizontalAlignment = HorizontalAlignment.Center
      };
      vBox.AddChild(label);

      var progress = new ProgressBar
      {
        MinValue = 0,
        MaxValue = 1,
        Value = 1,
        Modulate = cfg.TintColor,
        CustomMinimumSize = new Vector2(96, 12)
      };
      vBox.AddChild(progress);

      var key = kind switch
      {
        SkillKind.Dash => "Shift",
        SkillKind.Bomb => "Q",
        SkillKind.SlowMotion => "E",
        _ => ""
      };
      vBox.AddChild(new Label { Text = key, HorizontalAlignment = HorizontalAlignment.Center });

      _skillBar.AddChild(panel);

      var capturedKind = kind;
      var capturedCfg = cfg;
      c.OnSkillsStateChanged += state =>
      {
        if (state.Slots.TryGetValue(capturedKind, out var slot) && IsInstanceValid(progress))
        {
          progress.Value = slot.IsReady ? 1f : 1f - (slot.CooldownRemaining / capturedCfg.Cooldown);
        }
      };
    }
  }
}
