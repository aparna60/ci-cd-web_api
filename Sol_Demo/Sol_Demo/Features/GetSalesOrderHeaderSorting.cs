using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Sol_Demo.Features;

public enum OrderTypeEnum
{
    asc = 0,
    desc = 1
}

public class GetSalesOrderHeaderSortQueryRequestDTO
{
    public string? OrderByColumn { get; set; }

    public OrderTypeEnum OrderType { get; set; }
}

public class GetSalesOrderHeaderSortQueryResponseDTO
{
    public DateTime? OrderDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ShipDate { get; set; }
    public bool OnlineOrderFlag { get; set; }
    public string? SalesOrderNumber { get; set; }
}

public class GetSalesOrderHeaderSortQueryData : IRequest<List<GetSalesOrderHeaderSortQueryResponseDTO>>
{
    public GetSalesOrderHeaderSortQueryData(string? orderByColumn, OrderTypeEnum orderType)
    {
        this.OrderByColumn = orderByColumn;
        this.OrderType = orderType;
    }

    public string? OrderByColumn { get; }

    public OrderTypeEnum OrderType { get; }
}

public class GetSalesOrderHeaderSortQueryDataHandler : IRequestHandler<GetSalesOrderHeaderSortQueryData, List<GetSalesOrderHeaderSortQueryResponseDTO>>
{
    private readonly AdventureWorks2012Context adventureWorks2012Context;

    public GetSalesOrderHeaderSortQueryDataHandler(AdventureWorks2012Context adventureWorks2012Context)
    {
        this.adventureWorks2012Context = adventureWorks2012Context;
    }

    Task<List<GetSalesOrderHeaderSortQueryResponseDTO>> IRequestHandler<GetSalesOrderHeaderSortQueryData, List<GetSalesOrderHeaderSortQueryResponseDTO>>.Handle(GetSalesOrderHeaderSortQueryData request, CancellationToken cancellationToken)
    {
        IQueryable<SalesOrderHeader> query = this.adventureWorks2012Context.SalesOrderHeaders.AsQueryable().AsNoTracking();

        if (!query.Any())
            throw new Exception("Sales Order header data is not found");

        if (String.IsNullOrEmpty(request.OrderByColumn))
            throw new Exception("Order by Columns should not be empty");

        if (request.OrderType == null)
            throw new Exception("Order type should not be empty");

        var columnTemplate = $"{request.OrderByColumn} {request.OrderType.ToString().ToLower()}";

        if (request.OrderType == OrderTypeEnum.desc)
        {
            query = query.OrderBy(columnTemplate);
        }
        else if (request.OrderType == OrderTypeEnum.asc)
        {
            query = query.OrderBy(columnTemplate);
        }

        return query.Select((element) => new GetSalesOrderHeaderSortQueryResponseDTO()
        {
            DueDate = element.DueDate,
            OnlineOrderFlag = element.OnlineOrderFlag,
            OrderDate = element.OrderDate,
            SalesOrderNumber = element.SalesOrderNumber,
            ShipDate = element.ShipDate
        }).Take(5).ToListAsync(cancellationToken);
    }
}

public class GetSalesOrderHeaderSortQuery : GetSalesOrderHeaderSortQueryRequestDTO, IRequest<List<GetSalesOrderHeaderSortQueryResponseDTO>>
{
}

public class GetSalesOrderHeaderSortQueryHandler : IRequestHandler<GetSalesOrderHeaderSortQuery, List<GetSalesOrderHeaderSortQueryResponseDTO>>
{
    private readonly IMediator mediator;

    public GetSalesOrderHeaderSortQueryHandler(IMediator mediator)
    {
        this.mediator = mediator;
    }

    Task<List<GetSalesOrderHeaderSortQueryResponseDTO>> IRequestHandler<GetSalesOrderHeaderSortQuery, List<GetSalesOrderHeaderSortQueryResponseDTO>>.Handle(GetSalesOrderHeaderSortQuery request, CancellationToken cancellationToken)
    => mediator.Send(new GetSalesOrderHeaderSortQueryData(request.OrderByColumn, request.OrderType), cancellationToken);
}