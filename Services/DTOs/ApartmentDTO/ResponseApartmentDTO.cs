namespace Services.DTOs.ApartmentDTO
{
    public class ResponseApartmentDTO
    {
        public int ApartmentId { get; set; }

        public int? Area { get; set; }

        public string? Status { get; set; }

        public int Floor { get; set; }

        public string? Block { get; set; }
    }
}
