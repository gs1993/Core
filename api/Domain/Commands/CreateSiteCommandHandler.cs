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
        private readonly IDateTimeProvider _dateTimeProvider;

        public CreateSiteCommandHandler(DataContext context, IDateTimeProvider dateTimeProvider)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        public async Task<Result> Handle(CreateSiteCommand request, CancellationToken cancellationToken)
        {
            var createSiteAddressResult = Address.Create(request.AddressFirstLine, request.AddressSecondLine);
            if (createSiteAddressResult.IsFailure)
                return createSiteAddressResult;

            var createSiteResult = Site.Create(request.Name, createSiteAddressResult.Value, _dateTimeProvider.Now);
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
