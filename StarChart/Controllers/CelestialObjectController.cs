using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var result = _context.CelestialObjects.SingleOrDefault(x => x.Id == id);
            if (result == null)
            {
                return NotFound();
            }
            result.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == result.Id).ToList();
            return Ok(result);
        }

        [HttpGet("{name")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(x => x.Name == name);
            if (!celestialObjects.Any())
            {
                return NotFound();
            }
            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects
                    .Where(x => x.OrbitedObjectId == celestialObject.Id).ToList();
            }
            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects;
            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == celestialObject.Id).ToList();
            }
            return Ok(celestialObjects);
        }
    }
}
