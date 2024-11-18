using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class ManagementFee
{
    public int ManagementFeeId { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public DateOnly? DeletedDate { get; set; }

    public double? Price { get; set; }
}
