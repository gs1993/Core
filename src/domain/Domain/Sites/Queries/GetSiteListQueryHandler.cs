using Domain.Shared.DatabaseContext;
using Domain.Sites.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Dto.Site;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Sites.Queries
{
    public record GetSiteListQuery : IRequest<IReadOnlyList<SiteDto>>
    {
    }

    public class GetSiteListQueryHandler : IRequestHandler<GetSiteListQuery, IReadOnlyList<SiteDto>>
    {
        private readonly IReadonlyDataContext _readonlyDataContext;

        public GetSiteListQueryHandler(IReadonlyDataContext readonlyDataContext)
        {
            _readonlyDataContext = readonlyDataContext ?? throw new ArgumentNullException(nameof(readonlyDataContext));
        }

        public async Task<IReadOnlyList<SiteDto>> Handle(GetSiteListQuery request, CancellationToken cancellationToken)
        {
            var sites = await _readonlyDataContext
                .GetQuery<Site>()
                .ToListAsync(cancellationToken);

            return sites.Select(x => new SiteDto
            {
                Id = x.Id,
                Name = x.Name,
                AddressFirstLine = x.Address.FirstLine,
                AddressSecondLine = x.Address.SecondLine
            })?.ToList() ?? new List<SiteDto>();
        }
    }
}
