namespace Repositories.Entities;

public partial class Area
{
    public int AreaId { get; set; }

    public string AreaName { get; set; } = null!;

    public string? Location { get; set; }

    public virtual ICollection<Block> Blocks { get; set; } = new List<Block>();

    public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
}
