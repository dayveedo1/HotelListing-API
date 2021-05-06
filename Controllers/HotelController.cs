using AutoMapper;
using HotelListingAPI.Data;
using HotelListingAPI.Data.Dto;
using HotelListingAPI.Data.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("{hotelId:int}")]
        [Authorize]
        public async Task<IActionResult> GetHotelById(int hotelId)
        {
            try
            {
                var hotel = await unitOfWork.Hotels.Get(x => x.HotelId == hotelId);
                if (hotel == null)
                    return StatusCode(StatusCodes.Status404NotFound);

                var result = mapper.Map<HotelDto>(hotel);
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in {nameof(GetHotelById)}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Authorize(Roles ="ADMIN")]
        public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDto hotelDto)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError($"Cannot create record: {nameof(CreateHotel)}");
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }

            try
            {
                var hotel = mapper.Map<Hotel>(hotelDto);
                await unitOfWork.Hotels.Insert(hotel);
                await unitOfWork.Save();

                return StatusCode(StatusCodes.Status200OK, hotel);
            } catch(Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in {nameof(CreateHotel)}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Try again");
            }
        }

        [HttpPut("{HotelId:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateHotel(int HotelId, [FromBody] UpdateHotelDto hotelDto)
        {
            if (!ModelState.IsValid || HotelId < 1)
            {
                logger.LogError($"Invalid Update Atempt in {nameof(UpdateHotel)}");
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }

            try
            {
                var hotelToBeUpdated = await unitOfWork.Hotels.Get(x => x.HotelId == HotelId);
                if (hotelToBeUpdated == null)
                {
                    logger.LogError($"Invalid Update Atempt in {nameof(UpdateHotel)}");
                    return StatusCode(StatusCodes.Status404NotFound, $"Not Found");
                }

                mapper.Map(hotelDto, hotelToBeUpdated);
                unitOfWork.Hotels.Update(hotelToBeUpdated);
                await unitOfWork.Save();

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {

                logger.LogError(ex, $"Something went wrong in {nameof(UpdateHotel)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Please Try Again");
            }
        }

        [HttpDelete("{hotelId:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteHotel(int hotelId)
        {
            if (hotelId < 1)
            {
                logger.LogError($"Invalid DELETE Action in {nameof(DeleteHotel)}");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            try
            {
                var hotelToBeDeleted = await unitOfWork.Hotels.Get(x => x.HotelId == hotelId);
                if (hotelToBeDeleted == null)
                {
                    logger.LogError($"Invalid DELETE Attempt in {nameof(DeleteHotel)}");
                    return StatusCode(StatusCodes.Status404NotFound, $"Not Found");
                }

                await unitOfWork.Hotels.Delete(hotelId);
                await unitOfWork.Save();

                return StatusCode(StatusCodes.Status200OK, $"Deleted");
            }
            catch (Exception ex)
            {

                logger.LogError(ex, $"Something went wrong in {nameof(DeleteHotel)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Please Try Again");
            }
        }
    }
}
