using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Everbridge.ControlCenter.TechnicalChallenge.DoorDatabase
{
    public class DoorRepositoryService
    {
        private readonly DoorRepositoryDatabaseContext _userRepositoryDatabaseContext;

        public DoorRepositoryService(DoorRepositoryDatabaseContext userRepositoryDatabaseContext)
        {
            _userRepositoryDatabaseContext = userRepositoryDatabaseContext;
        }

        public async Task<List<string>> GetDoorsIds()
        {
            return _userRepositoryDatabaseContext.Doors.Select(x => x.Id).ToList();
        }

        public async Task<DoorRecordDto> GetDoor(string doorId)
        {
            var user = await _userRepositoryDatabaseContext.Doors.FindAsync(doorId);
            return (user != null) ? new DoorRecordDto(user) : null;
        }

        public async Task<DoorRecordDto> AddDoor(DoorRecordDto door)
        {
            var record = new DoorRecord
            {
                Label = door.Label,
                IsLocked = door.IsLocked,
                IsOpen = door.IsOpen
            };
            await _userRepositoryDatabaseContext.Doors.AddAsync(record);
            await _userRepositoryDatabaseContext.SaveChangesAsync();
            return new DoorRecordDto(record);
        }

        public async Task<DoorRecordDto> RemoveDoor(string doorId)
        {
            var record = await _userRepositoryDatabaseContext.Doors.FindAsync(doorId);
            if (record == null)
            {
                return null;
            }

            _userRepositoryDatabaseContext.Remove(record);
            await _userRepositoryDatabaseContext.SaveChangesAsync();

            return new DoorRecordDto(record);
        }

        /// <summary>
        /// Gets all the doors available
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DoorRecord>> GetAllDoors()
        {
            var doors = await _userRepositoryDatabaseContext.Doors.ToListAsync().ConfigureAwait(false);
            return doors;
        }

        /// <summary>
        /// Removes door on the basis of id
        /// </summary>
        /// <param name="doorID"></param>
        /// <returns></returns>
        public async Task<DoorRecord> GetDoorByID(string doorID)
        {
            var door = await _userRepositoryDatabaseContext.Doors.FirstOrDefaultAsync(d => d.Id == doorID).ConfigureAwait(false);

            return door;
        }

        public async Task<int> UpdateOpenStatus(string doorID,DoorRecordDto doorRecord)
        {
            int status = -1;
            var door = await _userRepositoryDatabaseContext.Doors.FirstOrDefaultAsync(d => d.Id == doorID).ConfigureAwait(false);

            if(door.IsOpen == false && door.IsLocked == false)  //Closed and unlocked
            {
                if(doorRecord.IsOpen == true)  //Asking to open. Can be done
                {
                    status = 0;
                    door.IsOpen = doorRecord.IsOpen;    //Open the door.
                    await _userRepositoryDatabaseContext.SaveChangesAsync().ConfigureAwait(false);
                }
                else if(doorRecord.IsLocked == true)    //Asking to lock. Can be locked
                {
                    status = 1;
                    door.IsLocked = doorRecord.IsLocked;    //Lock the door
                    await _userRepositoryDatabaseContext.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            else if(door.IsOpen == false && door.IsLocked == true)   //Closed and locked
            {
                if(doorRecord.IsOpen == true && doorRecord.IsLocked == false)   //Asking to unlock and open. Can be done.
                {
                    status = 2;
                    door.IsLocked = doorRecord.IsLocked;    //Unlock the door.
                    door.IsOpen = doorRecord.IsLocked;  //Open the door.

                    await _userRepositoryDatabaseContext.SaveChangesAsync().ConfigureAwait(false);
                }

                else if(doorRecord.IsOpen == true && doorRecord.IsLocked == true)   //Asking to open without unlocking. Cannot be opened unless it is unlocked.
                {
                    status = 3; //Return a message asking to unlock and then open.
                }                
            }
            else if (door.IsOpen == true && door.IsLocked == false) //Open and unlocked
            {
                if(doorRecord.IsOpen == false)  //Asking to close. Can be done.
                {
                    status = 4;
                    door.IsOpen = doorRecord.IsOpen;    //Close the door.

                    await _userRepositoryDatabaseContext.SaveChangesAsync().ConfigureAwait(false);
                }
                else if (doorRecord.IsOpen ==false && doorRecord.IsLocked == true)  //Asking to close and lock. Can be done.
                {
                    status = 5;
                    door.IsOpen = doorRecord.IsOpen;    //Close the door.
                    door.IsLocked = doorRecord.IsLocked;    //Lock the door.

                    await _userRepositoryDatabaseContext.SaveChangesAsync().ConfigureAwait(false);
                }
                else if(doorRecord.IsLocked == true)    //Asking to lock. Cannot be locked unless closed.
                {
                    status = 6; //Return a message asking to close and lock.
                }
            }

            else if(door.IsOpen == true && door.IsLocked == true)   //Open and locked. Not possible.
            {
                status = 7; //Return a message saying it is an invalid door. Need to be intervened.
            }

            else
            {
                status = 8;
            }

            return status;
        }
    }
}
