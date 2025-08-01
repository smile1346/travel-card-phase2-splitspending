using Microsoft.AspNetCore.Mvc;
using SplitManagement.DTOs;
using SplitManagement.Services;

namespace SplitManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SplitsController : ControllerBase
    {
        private readonly ISplitService _splitService;
        private readonly ISettlementService _settlementService;

        public SplitsController(ISplitService splitService, ISettlementService settlementService)
        {
            _splitService = splitService;
            _settlementService = settlementService;
        }

        [HttpPost]
        public async Task<ActionResult<SplitResponse>> CreateSplit([FromBody] CreateSplitRequest request)
        {
            try
            {
                var split = await _splitService.CreateSplitAsync(request);
                return CreatedAtAction(nameof(GetSplit), new { id = split.Id }, split);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SplitResponse>> GetSplit(Guid id)
        {
            var split = await _splitService.GetSplitByIdAsync(id);
            return split != null ? Ok(split) : NotFound();
        }

        [HttpGet("trip/{tripId}")]
        public async Task<ActionResult<IEnumerable<SplitResponse>>> GetSplitsByTrip(Guid tripId)
        {
            var splits = await _splitService.GetSplitsByTripIdAsync(tripId);
            return Ok(splits);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SplitResponse>> UpdateSplit(Guid id, [FromBody] CreateSplitRequest request)
        {
            try
            {
                var split = await _splitService.UpdateSplitAsync(id, request);
                return Ok(split);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSplit(Guid id)
        {
            await _splitService.DeleteSplitAsync(id);
            return NoContent();
        }

        [HttpPost("participants/{participantId}/mark-paid")]
        public async Task<IActionResult> MarkParticipantPaid(Guid participantId)
        {
            await _splitService.MarkParticipantPaidAsync(participantId);
            return Ok();
        }

        [HttpGet("settlement/{tripId}")]
        public async Task<ActionResult<IEnumerable<SettlementResponse>>> GetTripSettlements(Guid tripId)
        {
            var settlements = await _settlementService.GetTripSettlementsAsync(tripId);
            return Ok(settlements);
        }

        [HttpPost("settlement/calculate/{tripId}")]
        public async Task<ActionResult<IEnumerable<SettlementResponse>>> CalculateSettlements(Guid tripId)
        {
            var settlements = await _settlementService.CalculateSettlementsAsync(tripId);
            return Ok(settlements);
        }

        [HttpPost("settlement/{settlementId}/complete")]
        public async Task<ActionResult<SettlementResponse>> CompleteSettlement(Guid settlementId)
        {
            try
            {
                var settlement = await _settlementService.MarkSettlementCompletedAsync(settlementId);
                return Ok(settlement);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    // [ApiController]
    // [Route("api/[controller]")]
    // public class TagsController : ControllerBase
    // {
    //     private readonly ITagRepository _tagRepository;

    //     public TagsController(ITagRepository tagRepository)
    //     {
    //         _tagRepository = tagRepository;
    //     }

    //     [HttpPost]
    //     public async Task<ActionResult<TagResponse>> CreateTag([FromBody] CreateTagRequest request)
    //     {
    //         var tag = new Tag
    //         {
    //             Name = request.Name,
    //             Color = request.Color,
    //             Description = request.Description
    //         };

    //         var createdTag = await _tagRepository.CreateAsync(tag);
            
    //         var response = new TagResponse
    //         {
    //             Id = createdTag.Id,
    //             Name = createdTag.Name,
    //             Color = createdTag.Color,
    //             Description = createdTag.Description
    //         };

    //         return CreatedAtAction(nameof(GetTag), new { id = response.Id }, response);
    //     }

    //     [HttpGet("{id}")]
    //     public async Task<ActionResult<TagResponse>> GetTag(Guid id)
    //     {
    //         // Implementation would go here
    //         return NotFound();
    //     }
    }