using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StructProject.Database.Entities;

[Table("players")]
public class PlayerDB
{
  [Key]
  [Column("id")]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  [Required]
  required public int Id { get; set; }

  [Required]
  [Column("name")]
  required public string Name { get; set; }

  [Required]
  [Column("score")]
  required public int Score { get; set; }
}
