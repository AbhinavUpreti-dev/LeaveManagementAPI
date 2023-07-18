using AutoMapper;
using LeaveManagement.Application.Contracts;
using LeaveManagement.Application.Entity;
using LeaveManagement.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LeaveManagementAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class LeaveDetailsController : ControllerBase
    {
        public ILeaveRepository leaveRepository { get; }
        public IUserRepository UserRepository { get; }
        public IMapper Mapper { get; }

        public LeaveDetailsController(ILeaveRepository _leaveRepository,IUserRepository userRepository, IMapper _mapper)
        {
            leaveRepository = _leaveRepository;
            UserRepository = userRepository;
            Mapper = _mapper;
        }
        // GET: api/<LeaveDetailsController>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<LeaveDetails>>> Get()
        {
            var leaveDetails = await leaveRepository.GetAllLeaveDetails();
            var result = Mapper.Map<IEnumerable<LeaveDetails>>(leaveDetails);
            return Ok(result);
        }

        // GET api/<LeaveDetailsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveDetails>> Get(int id)
        {
            var leaveDetails = await leaveRepository.GetEntityById(id);
            var result = Mapper.Map<LeaveDetails>(leaveDetails);
            return Ok(result);
        }

        // POST api/<LeaveDetailsController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] LeaveDetails leaveDetails)
        {
            var leaveDetailsEntity = Mapper.Map<LeaveDetailsEntity>(leaveDetails);
            var anyValidUser = await UserRepository.AnyUserExists(leaveDetails.EmployeeId);
            if (anyValidUser == false)
            {
                throw new BadHttpRequestException("Invalid Employee Id");
                //return BadRequest("Invalid Employee Id");
            }
            else
            {
                
                leaveDetailsEntity.User = await UserRepository.GetEntityById(leaveDetails.EmployeeId);
            }
            leaveDetailsEntity.Status = "Pending Approval";
            await leaveRepository.CreateEntity(leaveDetailsEntity);
            return  CreatedAtAction(nameof(Get), new { id = leaveDetailsEntity.Id }, null);
        }

        // PUT api/<LeaveDetailsController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] LeaveDetails leaveDetails)
        {
            var leaveDetailsEntity = Mapper.Map<LeaveDetailsEntity>(leaveDetails);
            await leaveRepository.UpdateEntity(leaveDetailsEntity);
            return Ok();

        }

        // DELETE api/<LeaveDetailsController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var leaveDetailsEntity = await leaveRepository.GetEntityById(id);
                await leaveRepository.DeleteEntity(leaveDetailsEntity);
                return Ok();
            }
            catch(WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest("Invalid Request");
            }

        }
    }
}
