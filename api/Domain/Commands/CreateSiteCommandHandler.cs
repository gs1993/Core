using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Entities.Site;
using WebApi.Helpers;

namespace WebApi.Domain.Commands
{
    public record CreateSiteCommand : IRequest<Result>
    {
        public string Name { get; init; }
        public string AddressFirstLine { get; init; }
        public string AddressSecondLine { get; init; }
    }

    public class CreateSiteCommandHandler : IRequestHandler<CreateSiteCommand, Result>
    {
        private readonly DataContext _context;

        public CreateSiteCommandHandler(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(CreateSiteCommand request, CancellationToken cancellationToken)
        {
            var createSiteAddressResult = Address.Create(request.AddressFirstLine, request.AddressSecondLine);
            if (createSiteAddressResult.IsFailure)
                return Result.Failure(createSiteAddressResult.Error);

            var createSiteResult = Site.Create(request.Name, createSiteAddressResult.Value);
            if (createSiteResult.IsFailure)
                return Result.Failure(createSiteResult.Error);
            var newSite = createSiteResult.Value;

            var siteWithThatNameAlreadyExists = await _context.Sites
                .AnyAsync(x => x.Name == newSite.Name, cancellationToken: cancellationToken);
            if (siteWithThatNameAlreadyExists)
                return Result.Failure("Site with that name already exists");

            await _context.Sites.AddAsync(newSite, cancellationToken);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}
