using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace NextVibe.Models
{
    public class ArtistMoodSelectionViewModel
    {
        [Required]
        [Display(Name = "Artist")]
        public int ArtistId { get; set; }

        public List<int> SelectedMoodIds { get; set; } = new();

        public List<SelectListItem> Artists { get; set; } = new();
        public List<SelectListItem> Moods { get; set; } = new();
    }
}