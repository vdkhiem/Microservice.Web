using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.WebApi.Interfaces;
using Microservice.WebAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/advert")]
    public class Advert : ControllerBase
    {
        private readonly IAdvertStorageService service;

        public Advert(IAdvertStorageService service)
        {
            this.service = service;
        }

        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(404)]
        [ProducesResponseType(201, Type=typeof(CreateAdvertResponse))]
        public async Task<IActionResult> Create(AdvertModel model)
        {
            string recordId = string.Empty;
            try
            {
                recordId = await service.Add(model);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            return StatusCode(201, new CreateAdvertResponse() { Id = recordId });
        }

        [HttpPost]
        [Route("Confirm")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Confirm(ConfirmAdvertModel model)
        {
            try
            {
                await service.Confirm(model);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception e)
            {

                return StatusCode(500, e.Message);

            }
            return StatusCode(201);
        }
    }
}