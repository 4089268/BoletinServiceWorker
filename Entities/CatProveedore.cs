using System;
using System.Collections.Generic;

namespace BoletinServiceWorker.Entities;

public partial class CatProveedore
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;
}
