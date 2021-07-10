using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Everbridge.ControlCenter.TechnicalChallenge.DoorDatabase;
using Everbridge.ControlCenter.TechnicalChallenge.Models;

namespace Everbridge.ControlCenter.TechnicalChallenge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DoorController : ControllerBase
    {
        private readonly ILogger<DoorController> _logger;
        private readonly DoorRepositoryService _doorRepositoryService;

        public DoorController(ILogger<DoorController> logger, DoorRepositoryDatabaseContext databaseContext)
        {
            _logger = logger;
            _doorRepositoryService = new DoorRepositoryService(databaseContext);
        }

        //[HttpGet]
        //public async Task<IEnumerable<string>> Get()
        //{
        //    return await _doorRepositoryService.GetDoorsIds();
        //}

        [HttpGet]
        [Route("{doorId}")]
        public async Task<DoorModel> GetDoor([FromRoute] [Required] string doorId)
        {
            var doorRecord = await _doorRepositoryService.GetDoor(doorId);

            return (doorRecord == null)
                ? null
                : new DoorModel
                {
                    Id = doorRecord.Id,
                    Label = doorRecord.Label,
                    IsOpen = doorRecord.IsOpen,
                    IsLocked = doorRecord.IsLocked
                };
        }
        /// <summary>
        /// Adds a door and returns a message
        /// </summary>
        /// <param name="door"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ObjectResult> AddDoor([FromBody][Required] DoorRecordDto door)
        {
            if(door!=null)
            {
                await _doorRepositoryService.AddDoor(door);

                return Ok("Door added successfully");
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }

        /// <summary>
        /// Returns all the available doors
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ObjectResult> GetAllDoors()
        {
            var doorList = await _doorRepositoryService.GetAllDoors().ConfigureAwait(false);
            if(!doorList.Any() || doorList == null)
            {
                return NotFound("No doors available");
            }
            else
            {
                return Ok(doorList);
            }
        }

        /// <summary>
        /// Removes door on the basis of door id
        /// </summary>
        /// <param name="doorId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{doorId}")]
        public async Task<ObjectResult> RemoveDoor([FromRoute] string doorId)
        {
            var door = await _doorRepositoryService.GetDoorByID(doorId).ConfigureAwait(false);

            if(door != null)
            {
                await _doorRepositoryService.RemoveDoor(doorId).ConfigureAwait(false);

                return Ok($"Door with {doorId} has been deleted.");
            }

            return NotFound($"Door with {doorId} not found.");
        }

        /// <summary>
        /// Closes a door.
        /// </summary>
        /// <param name="doorID"></param>
        /// <param name="doorRecord"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{doorId}")]
        public async Task<ObjectResult> UpdateDoor([FromRoute]string doorID, [FromBody] DoorRecordDto doorRecord)
        {
            var door = await _doorRepositoryService.GetDoorByID(doorID);

            if (door != null)
            {
                var status = await _doorRepositoryService.UpdateOpenStatus(doorID, doorRecord);

                switch(status)
                {
                    case 0:
                        return Ok($"Door {doorID} has been opened.");
                    case 1:
                        return Ok($"Door {doorID} has been locked.");
                    case 2:
                        return Ok($"Door {doorID} has been unlocked and opened.");
                    case 3:
                        return BadRequest($"Door {doorID} has to be unlocked first to open.");
                    case 4:
                        return Ok($"Door {doorID} has been closed.");
                    case 5:
                        return Ok($"Door {doorID} has been closed and locked.");
                    case 6:
                        return BadRequest($"Door {doorID} has to be closed to be locked.");
                    case 7:
                        return BadRequest($"Door {doorID} is invalid. Need intervention.");
                    default:
                        return NotFound("Bad Request.");
                }
            }
            else
            {
                return NotFound($"Door with id {doorID} not found.");
            }
        }
    }
}
