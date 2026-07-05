using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StructProject.Database.Entities;

[Table("players_x")]
public class PlayerProfileDB
{
  [Key]
  [Column("id")]
  [Required]
  public int Id { get; set; } = 1;

  [Required]
  [Column("coins")]
  public int Coins { get; set; }

  [Required]
  [Column("best_wave")]
  public int BestWave { get; set; }

  [Required]
  [Column("best_score")]
  public int BestScore { get; set; }

  [Required]
  [Column("extra_max_hp")]
  public int ExtraMaxHp { get; set; }

  [Required]
  [Column("damage_level")]
  public int DamageLevel { get; set; }

  [Required]
  [Column("speed_level")]
  public int SpeedLevel { get; set; }

  [Required]
  [Column("last_played_at")]
  public DateTime LastPlayedAt { get; set; } = DateTime.UtcNow;
}
