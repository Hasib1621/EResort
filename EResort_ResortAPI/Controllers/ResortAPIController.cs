using EResort_ResortAPI.Data;
using EResort_ResortAPI.Models;
using EResort_ResortAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EResort_ResortAPI.Controllers
{
    [Route("api/ResortAPI")]
    [ApiController]
    public class ResortAPIController : ControllerBase
    {
        private readonly ILogger<ResortAPIController> _logger;
        private readonly ApplicationDbContext _db;

        public ResortAPIController(ILogger<ResortAPIController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ResortDTO>> GetResorts()
        {
            _logger.LogInformation("Getting All Resorts");
            //return Ok(ResortStore.resortList);
            return Ok(_db.Resorts.ToList());
        }

        [HttpGet("{id:int}", Name ="GetResort")]
        //[ProducesResponseType(200, Type =typeof(ResortDTO))]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(400)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ResortDTO> GetResort(int id)
        {
            if(id == 0)
            {
                _logger.LogError("Get resort error with ID " + id);
                return BadRequest();
            }
            //var resort = ResortStore.resortList.FirstOrDefault(u => u.Id == id);
            var resort = _db.Resorts.FirstOrDefault(u => u.Id == id);
            if (resort == null)
            {
                return NotFound();
            }
            return Ok(resort);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ResortDTO> CreateResort([FromBody]ResortDTO dto)
        {
            if(_db.Resorts.FirstOrDefault(u=>u.Name.ToLower() == dto.Name.ToLower()) != null)
            {
                ModelState.AddModelError("customError","Resort already exists");
                return BadRequest(ModelState);
            }
            if(dto == null)
            {
                return BadRequest(dto);
            }
            if(dto.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            //dto.Id = ResortStore.resortList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
            //ResortStore.resortList.Add(dto);

            Resort model = new()
            {
                Amenity = dto.Amenity,
                Details = dto.Details,
                ImageUrl = dto.ImageUrl,
                Name = dto.Name,
                Occupancy = dto.Occupancy,
                Rate = dto.Rate,
                Sqft = dto.Sqft
            };
            _db.Resorts.Add(model);
            _db.SaveChanges();

            return CreatedAtRoute("GetResort", new {id = dto.Id} , dto);
        }

        [HttpDelete("{id:int}", Name ="DeleteResort")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteResort(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            //var resort = ResortStore.resortList.FirstOrDefault(u => u.Id == id);
            var resort = _db.Resorts.FirstOrDefault(u => u.Id == id);
            if (resort == null)
            {
                return NotFound();
            }
            //ResortStore.resortList.Remove(resort);
            _db.Resorts.Remove(resort); 
            _db.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateResort")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateResort(int id, [FromBody] ResortDTO dto)
        {
            if(dto == null || id != dto.Id)
            {
                return BadRequest();
            }
            //var resort = ResortStore.resortList.FirstOrDefault(u => u.Id == id);
            //resort.Name = resortDTO.Name;
            //resort.Sqft = resortDTO.Sqft;
            //resort.Occupancy = resortDTO.Occupancy;

            Resort model = new()
            {
                Id = id,
                Amenity = dto.Amenity,
                Details = dto.Details,
                ImageUrl = dto.ImageUrl,
                Name = dto.Name,
                Occupancy = dto.Occupancy,
                Rate = dto.Rate,
                Sqft = dto.Sqft
            };

            _db.Resorts.Update(model);
            _db.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialResort")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialResort(int id, JsonPatchDocument<ResortDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            //var resort = ResortStore.resortList.FirstOrDefault(u => u.Id == id);
            var resort = _db.Resorts.AsNoTracking().FirstOrDefault(u => u.Id == id);

            ResortDTO resortDTO = new()
            {
                Id = resort.Id,
                Amenity = resort.Amenity,
                Details = resort.Details,
                ImageUrl = resort.ImageUrl,
                Name = resort.Name,
                Occupancy = resort.Occupancy,
                Rate = resort.Rate,
                Sqft = resort.Sqft
            };

            if (resort == null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(resortDTO, ModelState);
            Resort model = new()
            {
                Id = resortDTO.Id,
                Amenity = resortDTO.Amenity,
                Details = resortDTO.Details,
                ImageUrl = resortDTO.ImageUrl,
                Name = resortDTO.Name,
                Occupancy = resortDTO.Occupancy,
                Rate = resortDTO.Rate,
                Sqft = resortDTO.Sqft
            };

            _db.Resorts.Update(model);
            _db.SaveChanges();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return NoContent();
        }



    }
}
