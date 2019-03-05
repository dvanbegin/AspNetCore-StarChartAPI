using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

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

        [HttpGet("{name}")]
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

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CelestialObject celestialObject)
        {
            var celestialObjectToUpdate = _context.CelestialObjects.Find(id);
            if (celestialObjectToUpdate == null)
            {
                return NotFound();
            }
            celestialObjectToUpdate.Name = celestialObject.Name;
            celestialObjectToUpdate.OrbitedObjectId = celestialObject.OrbitedObjectId;
            celestialObjectToUpdate.OrbitalPeriod = celestialObject.OrbitalPeriod;
            _context.Update(celestialObjectToUpdate);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var objectToRename = _context.CelestialObjects.Find(id);
            if (objectToRename == null)
            {
                return NotFound();
            }

            objectToRename.Name = name;
            _context.CelestialObjects.Update(objectToRename);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objectsToDelete = _context.CelestialObjects.Where(x=>x.Id==id || x.OrbitedObjectId == id);
            if (!objectsToDelete.Any())
            {
                return NotFound();
            }
            _context.CelestialObjects.RemoveRange(objectsToDelete);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
