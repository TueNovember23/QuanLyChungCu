using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class WaterFeeHistory
{
    public DateOnly? CreatedDate { get; set; }

    public DateOnly? DeletedDate { get; set; }

    public int? Level1 { get; set; }

    public double? Price1 { get; set; }

    public int? Level2 { get; set; }

    public double? Price2 { get; set; }

    public int? Level3 { get; set; }

    public double? Price3 { get; set; }
}
