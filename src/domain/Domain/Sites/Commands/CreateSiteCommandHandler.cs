using CSharpFunctionalExtensions;
using Domain.Shared.DatabaseContext;
using Domain.Sites.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Sites.Commands
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
                return createSiteAddressResult;

            var createSiteResult = Site.Create(request.Name, createSiteAddressResult.Value);
            if (createSiteResult.IsFailure)
                return createSiteResult;
            var newSite = createSiteResult.Value;

            var siteWithThatNameAlreadyExists = await _context.Sites
                .AnyAsync(x => x.Name == newSite.Name, cancellationToken: cancellationToken);
            if (siteWithThatNameAlreadyExists)
                return Result.Failure("Site with that name already exists");

            _context.Sites.Attach(newSite);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
