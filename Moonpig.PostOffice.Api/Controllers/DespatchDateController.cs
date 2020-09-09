namespace Moonpig.PostOffice.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    [Route("api/[controller]")]
    public class DespatchDateController : Controller
    {
        private readonly IMediator mediator;

        public DespatchDateController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public DespatchDate GetDespatchDate(List<int> productIds, DateTime orderDate)
        {
            return mediator.Send(new Order(){ OrderDate = orderDate, ProductIds = productIds }).Result;
        }
    }
}
