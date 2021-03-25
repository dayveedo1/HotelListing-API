using AutoMapper;
using HotelListingAPI.Data.Dto;
using HotelListingAPI.Data.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : Controller
    {

        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<HotelController> logger;
        private readonly IMapper mapper;

        public HotelController(IUnitOfWork unitOfWork, ILogger<HotelController> logger, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHotels()
        {
            try
            {
                var hotels = await unitOfWork.Hotels.GetAll();
                var result = mapper.Map<IList<HotelDto>>(hotels);
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong: {nameof(GetAllHotels)}");
                //return StatusCode(500, "Internal Server Error");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetHotelById(int id)
        {
            try
            {
                var hotel = await unitOfWork.Hotels.Get(x => x.HotelId == id);
                var result = mapper.Map<HotelDto>(hotel);
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in {nameof(GetHotelById)}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
