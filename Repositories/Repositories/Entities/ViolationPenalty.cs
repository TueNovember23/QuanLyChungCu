using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class ViolationPenalty
{
    public int PenaltyId { get; set; }

    public int ViolationId { get; set; }

    public string? PenaltyLevel { get; set; }

    public decimal? Fine { get; set; }

    public string? PenaltyMethod { get; set; }

    public string? ProcessingStatus { get; set; }

    public DateTime? ProcessedDate { get; set; }

    public string? Note { get; set; }

    public virtual Violation Violation { get; set; } = null!;
}
