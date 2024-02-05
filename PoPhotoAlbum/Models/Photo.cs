using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PoPhotoAlbum.Models
{
    public class Photo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PhotoId { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string Url { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime UploadDate { get; set; }

        // Additional metadata fields can be added here
        // public string Description { get; set; }
        // public string Tags { get; set; } // Could be a comma-separated values or a JSON string
    }
}
