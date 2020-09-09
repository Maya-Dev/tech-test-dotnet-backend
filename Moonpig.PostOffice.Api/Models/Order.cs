namespace Moonpig.PostOffice.Api.Models
{
    using System;
    using System.Collections.Generic;
    using MediatR;

    public class Order : IRequest<DespatchDate>
    {
        public List<int> ProductIds { get; set; }
        public DateTime OrderDate { get; set; }
    }
}