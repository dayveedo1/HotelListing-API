using AutoMapper;
using HotelListingAPI.Data.Dto;
using HotelListingAPI.Data.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<CountryController> logger;
        private readonly IMapper mapper;

        public CountryController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpGet]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCountries()
        {
            try
            {
                var countries = await unitOfWork.Countries.GetAll();
                var result = mapper.Map<IList<CountryDto>>(countries);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong: {nameof(GetAllCountries)}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{id:int}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCountryById(int id)
        {
            try
            {
                var country = await unitOfWork.Countries.Get(q => q.CountryId == id, 
                                new List<string> { "Hotels" });
                var result = mapper.Map<CountryDto>(country);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in {nameof(GetCountryById)}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
