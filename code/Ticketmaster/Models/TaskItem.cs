using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ticketmaster.Models
{
    public class TaskItem
    {
        [Key]
        public int TaskItemId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string? Description { get; set; }

        public int StageId { get; set; }

        [ForeignKey("StageId")]
        public Stage Stage { get; set; }

        // Optional metadata
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsComplete { get; set; } = false;
    }
}