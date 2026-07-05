using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StructProject.Database.Entities;

[Table("save_slots")]
public class SaveSlotDB
{
  [Key]
  [Column("slot")]
  [Required]
  public int Slot { get; set; }

  [Required]
  [Column("name")]
  [MaxLength(16)]
  public string Name { get; set; } = "Player";

  [Required]
  [Column("json_state")]
  public string JsonState { get; set; } = "{}";

  [Required]
  [Column("created_at")]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  [Required]
  [Column("updated_at")]
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
