using System;

namespace GS1ProductTracker.Shared.DTO.ModelRow
{
    public class ModelRowImageDTO
    {
        public long ID { get; set; }
        public Guid? BinaryDataID { get; set; }
        public byte[] BinaryData { get; set; }
        public string BinaryBase64 { get; set; }
        public byte[] ThumbnailBinaryData { get; set; }
        public string ThumbnailBinaryBase64 { get; set; }
        public string Filename { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? Filesize { get; set; }
        public string MD5 { get; set; }
        public string MIMEType { get; set; }
        public string URI { get; set; }
        public DateTime? StartProcessingOn { get; set; }
        public DateTime? EndProcessingOn { get; set; }
        public string Error { get; set; }
        public long ModelRowId { get; set; }
    }
}
