using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataService.Http;

namespace PlatformService.Controllers
{
    // api/Platforms, it leaves off the controller part
    [Route("api/[controller]")]
    // This allows you to type in http//...../2
    // With 2 being the id
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(
            IPlatformRepo repository, 
            IMapper mapper,
            ICommandDataClient commandDataClient,
            IMessageBusClient messageBusClient
            )
        {
            _repository = repository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("--> Getting Platforms");

            var platformItem = _repository.GetAllPlatforms();

            // We're mapping to IEnumerable<PlatformReadDto from platformItem
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItem));
        }

        // If you don't add the id part, it won't know which
        // HttpGet to get
        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            var platformItem = _repository.GetPlatformById(id);
            if (platformItem != null){
                return Ok(_mapper.Map<PlatformReadDto>(platformItem));
            }

            // This returns 404
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatformAsync(PlatformCreateDto platformCreateDto)
        {
            var platformModel = _mapper.Map<Platform>(platformCreateDto);
            _repository.CreatePlatform(platformModel);
            _repository.SaveChanges();

            var PlatformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            // Synd Sync Message
            try
            {
                await _commandDataClient.SendPlatformToCommand(PlatformReadDto);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
            }

            // Synd Async Message
            try
            {
                var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(PlatformReadDto);
                platformPublishedDto.Event = "Platform_Published";
                _messageBusClient.PublishNewPlatform(platformPublishedDto);
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
            }

            // This returns uri (where you can find this created object)
            return CreatedAtRoute(nameof(GetPlatformById), new { Id = PlatformReadDto.Id }, PlatformReadDto);
        }
    }
}