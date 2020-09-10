namespace Moonpig.PostOffice.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Moonpig.PostOffice.Api.Attributes;

    [ApiController]
    [Route("api/[controller]")]
    public class DespatchDateController : ControllerBase
    {
        private readonly IMediator mediator;

        public DespatchDateController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public ActionResult<DespatchDate> GetDespatchDate([FromQuery][NonEmptyList]List<int> productIds, [FromQuery][Required]DateTime? orderDate)
        {
            var despatchDate = mediator.Send(new Order(){ OrderDate = (DateTime)orderDate, ProductIds = productIds }).Result;
            if(despatchDate == null)
            {
                return NotFound();
            }
            return despatchDate;
        }
    }
}
