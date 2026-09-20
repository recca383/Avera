using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avera.Domain.Application.CaseImages;
using Avera.Domain.Application.Cases;
using Avera.Domain.Application.ExportedReports;
using Avera.Domain.Application.Notifications;
using Avera.Domain.Application.OverlayImages;
using Microsoft.EntityFrameworkCore;

namespace Avera.Application.Abstractions.Databases
{
    public interface IApplicationDbContext
    {
        DbSet<Case> Cases { get; set; }
        DbSet<CaseImage> CaseImages { get; set; }
        DbSet<ExportedReport> ExportedReports {get; set; }
        DbSet<Notification> Notifications { get; set; }
        DbSet<GradCamImage> GradCamImages { get; set; }
        DbSet<Avera.Domain.Application.CaseViews.CaseView> CaseViews { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
