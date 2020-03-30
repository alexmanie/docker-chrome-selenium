using System.Collections.Generic;

namespace GS1ProductTracker.Shared.DTO.Model
{
    public class ModelRowEditDeserializedDTO
    {
        public int EntityId { get; set; }
        public string Description { get;set;}
        public string GTIN { get; set; }
        public string GLN { get; set; }
        public string InternalCode { get; set; }
        public string ContentBasePath { get; set; }
        public string ContentFullPath { get; set; }
        public string Company { get; set; }
        public ImageDeserializedDTO Image { get; set; }
        public string ImageFullPath { get; set; }

        public List<TextualDataDeserializedDTO> TextualData { get; set; }
    }

    public class ImageDeserializedDTO
    {
        public string FileName { get; set; }
        public string FileBase64 { get; set; }
        public int FileSize { get; set; }
    }

    public class TextualDataDeserializedDTO
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }
}
