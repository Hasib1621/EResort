using EResort_ResortAPI.Data;
using EResort_ResortAPI.Models;
using EResort_ResortAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EResort_ResortAPI.Controllers
{
    [Route("api/ResortAPI")]
    [ApiController]
    public class ResortAPIController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ResortDTO>> GetResorts()
        {
            return Ok(ResortStore.resortList);
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
                return BadRequest();
            }
            var resort = ResortStore.resortList.FirstOrDefault(u => u.Id == id);
            if(resort == null)
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
            if(ResortStore.resortList.FirstOrDefault(u=>u.Name.ToLower() == dto.Name.ToLower()) != null)
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

            dto.Id = ResortStore.resortList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
            ResortStore.resortList.Add(dto);
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
            var resort = ResortStore.resortList.FirstOrDefault(u => u.Id == id);
            if(resort == null)
            {
                return NotFound();
            }
            ResortStore.resortList.Remove(resort);
            return NoContent();
        }



    }
}
