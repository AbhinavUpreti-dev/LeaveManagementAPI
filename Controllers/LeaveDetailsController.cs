using AutoMapper;
using LeaveManagement.Application.Contracts;
using LeaveManagement.Core.Entity;
using LeaveManagement.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using System.Net;
using MediatR;
using LeaveManagement.Application.UseCases.Queries;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LeaveManagementAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class LeaveDetailsController : ControllerBase
    {
        private readonly ISieveProcessor _sieveProcessor;
        private readonly IMediator _mediator;

        public ILeaveRepository leaveRepository { get; }
        public IUserRepository UserRepository { get; }
        public IMapper Mapper { get; }

        public LeaveDetailsController(ILeaveRepository _leaveRepository,IUserRepository userRepository, IMapper _mapper, ISieveProcessor sieveProcessor,IMediator mediator)
        {
            leaveRepository = _leaveRepository;
            UserRepository = userRepository;
            Mapper = _mapper;
            _sieveProcessor = sieveProcessor;
            this._mediator = mediator;
        }
        // GET: api/<LeaveDetailsController>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<LeaveDetails>>> Get([FromQuery] SieveModel sieveModel)
        {
            var result = await this._mediator.Send(new GetLeaveDetailsQuery { sieveModel=sieveModel});
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
