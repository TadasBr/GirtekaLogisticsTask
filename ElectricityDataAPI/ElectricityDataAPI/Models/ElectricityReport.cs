using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ElectricityDataAPI.Models
{
    public sealed class RealEstate
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [JsonIgnore]
        public int ObjectNumber { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string Region { get; set; }
        [Column(TypeName = "tinyint")]
        public HouseType HouseType { get; set; }
        [Column(TypeName = "tinyint")]
        public ObjectType ObjectType { get; set; }
        [JsonIgnore]
        public ICollection<ElectricityReport> ElectricityReports { get; set; }

        public override int GetHashCode() => ObjectNumber.GetHashCode();
        public override bool Equals(object? obj) => obj is RealEstate estate && estate.ObjectNumber == ObjectNumber;
        public override string ToString() => $"{ObjectNumber} in \"{Region}\" of type \"{HouseType}\" being \"{ObjectType}\"";
    }

    public sealed class ElectricityReport
    {
        [JsonIgnore, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public float? ConsumedElectricity { get; set; }
        public float? GeneratedElectricity { get; set; }
        [Column(TypeName = "datetime2(0)")] 
        public DateTime Time { get; set; }
        public int RealEstateObjectNumber { get; set; }
        [ForeignKey(nameof(RealEstateObjectNumber))]
        public RealEstate RealEstate { get; set; }
        public override bool Equals(object? obj) => obj is ElectricityReport report && report.Id == Id;
        public override string ToString() => 
            $"[{Time}]: P+:{ConsumedElectricity?.ToString("N2") ?? "N/A"}, P-{GeneratedElectricity?.ToString("N2") ?? "N/A"}";
    }

    public enum HouseType : byte
    {
        House, Apartment
    }

    public enum ObjectType : byte
    {
        ProducingConsumer, RemoteProducingConsumer, DomesticUser
    }
}
