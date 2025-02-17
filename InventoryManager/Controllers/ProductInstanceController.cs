﻿using InventoryManager.Core.DTO;
using InventoryManager.Core.Interfaces;
using InventoryManager.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace InventoryManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductInstanceController : ControllerBase
    {
        private readonly IProductInstanceService _productInstanceService;
       public ProductInstanceController(IProductInstanceService productInstanceService) 
        { 
            _productInstanceService = productInstanceService;
        }


        //Create
        [HttpPost]
        public async Task<IActionResult> CreateProductInstance([FromBody] ProductInstanceCreateRequest productInstanceCreateRequest)
        {
            if (productInstanceCreateRequest == null)
            {
                return BadRequest();
            }

            var result = await _productInstanceService.CreateProductInstance(productInstanceCreateRequest);

            if(result.IsSuccess) 
            { 
                return Ok(result); 
            }else
            {
                return BadRequest(new {Success = "False", Error = result.Error});
            }

        }
        //Read
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductInstance(string? id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return BadRequest(new { error = "Id must be provided" });
            }

            //get instance:
            var response = await _productInstanceService.GetProductInstanceById(id);

            if(!response.IsSuccess)
            {
                return NotFound(new { Error = response.Error});
            }

            return Ok(new { ProductInstance = response.Value });

        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductInstanceGetRequest productInstanceGetRequest)
        {
            if(productInstanceGetRequest == null)
            {  return BadRequest(new {Error = "Get request cannot be null."}); }

            var result = await _productInstanceService.GetAllProductInstances(productInstanceGetRequest);

            if(!result.IsSuccess)
            {
                return BadRequest(new {Error = result.Error});
            }

            return Ok(new {ProductInstances = result.Value});

        }

        //Update
        [HttpPut]
        public async Task<IActionResult> Put()
        {
            return Ok();
        }

        //Delete

    }
}
