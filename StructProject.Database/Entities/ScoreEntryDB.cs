using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StructProject.Database.Entities;

[Table("score_entries_x")]
public class ScoreEntryDB
{
  [Key]
  [Column("id")]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  [Required]
  public int Id { get; set; }

  [Required]
  [Column("name")]
  [MaxLength(16)]
  public string Name { get; set; } = "Player";

  [Required]
  [Column("score")]
  public int Score { get; set; }

  [Required]
  [Column("wave")]
  public int Wave { get; set; }

  [Required]
  [Column("created_at")]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
