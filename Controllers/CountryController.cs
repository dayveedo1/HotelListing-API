using AutoMapper;
using HotelListingAPI.Data;
using HotelListingAPI.Data.Dto;
using HotelListingAPI.Data.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateCountry([FromBody] CreateCountryDto countryDto)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError($"Cannot save record for {nameof(CreateCountry)}");
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }
            try
            {
                var country = mapper.Map<Country>(countryDto);
                await unitOfWork.Countries.Insert(country);
                await unitOfWork.Save();

                return StatusCode(StatusCodes.Status200OK, country);
            }
            catch (Exception ex)
            {

                logger.LogError(ex, $"Something went wrong in {nameof(CreateCountry)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Please Try Again");
            }
        }

        [HttpPut("{CountryId:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateCountry(int CountryId, [FromBody] UpdateCountryDto updateCountryDto)
        {
            if (!ModelState.IsValid || CountryId < 1)
            {
                logger.LogError($"Inavlid Update Attempt in {nameof(UpdateCountry)}");
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }

            try
            {
                var countryToBeUpdated = await unitOfWork.Countries.Get(x => x.CountryId == CountryId);
                if (countryToBeUpdated == null)
                {
                    logger.LogError($"Invalid Update Attempt in {nameof(UpdateCountry)}");
                    return StatusCode(StatusCodes.Status404NotFound, $"Not Found");
                }

                mapper.Map(updateCountryDto, countryToBeUpdated);
                unitOfWork.Countries.Update(countryToBeUpdated);
                await unitOfWork.Save();

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {

                logger.LogError(ex, $"Something went wrong in {nameof(UpdateCountry)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Please Try again");
            }
            
        }
    }
}
