using System;

namespace GS1ProductTracker.Shared.DTO.Entities
{
    public class EntitiesDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GLN { get; set; }
        public bool HasWebScrapper { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}
