using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Dto.Site;
using WebApi.Helpers;

namespace WebApi.Domain.Queries
{
    public record GetSiteListQuery : IRequest<IReadOnlyList<SiteDto>>
    {
        public string Name { get; init; }
    }

    public class GetSiteListQueryHandler : IRequestHandler<GetSiteListQuery, IReadOnlyList<SiteDto>>
    {
        private readonly QueriesConnectionString _connectionString;

        public GetSiteListQueryHandler(QueriesConnectionString connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IReadOnlyList<SiteDto>> Handle(GetSiteListQuery request, CancellationToken cancellationToken)
        {
            string query = $"SELECT Id, Name, AddressFirstLine, AddressSecondLine FROM Sites";
            using var connection = new SqlConnection(_connectionString.Value);
            var result = await connection.QueryAsync<SiteDto>(query);
            return result.ToList();
        }
    }
}
