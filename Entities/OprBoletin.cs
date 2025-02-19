using System;
using System.Collections.Generic;

namespace BoletinServiceWorker.Entities;

public partial class OprBoletin
{
    public Guid Id { get; set; }

    public string Titulo { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? FinishedAt { get; set; }

    public virtual ICollection<BoletinMensaje> BoletinMensajes { get; set; } = new List<BoletinMensaje>();

    public virtual ICollection<Destinatario> Destinatarios { get; set; } = new List<Destinatario>();
}
