namespace Services.DTOs.ApartmentDTO
{
    public class ResponseApartmentDTO
    {
        public string? Block { get; set; }
        public int Floor { get; set; }

        public string ApartmentCode { get; set; } = null!;

        public int? Area { get; set; }

        public int? NumberOfPeople { get; set; }

        public string? Status { get; set; }
    }
}
