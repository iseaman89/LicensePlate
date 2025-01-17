using AutoMapper;
using LicensePlateDataShared.Models;
using LicensePlateDataShared.Static;
using LicensePlateServer.Models.LicensePlates;
using LicensePlateServer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LicensePlateServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LicensePlateController : ControllerBase
    {
        private readonly ILicensePlateRepository _licensePlateRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<LicensePlateController> _logger;

        public LicensePlateController(ILicensePlateRepository licensePlateRepository, IMapper mapper, ILogger<LicensePlateController> logger)
        {
            _licensePlateRepository = licensePlateRepository;
            _mapper = mapper;
            _logger = logger;
        }
        
        /// <summary>
        /// Gets all license plates.
        /// </summary>
        /// <returns>A list of license plates.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LicensePlateReadOnlyDto>>> GetLicensePlates()
        {
            try
            {
                var licensePlates = await _licensePlateRepository.GetAllAsync();
                var licensePlateDtos = _mapper.Map<IEnumerable<LicensePlateReadOnlyDto>>(licensePlates);
                return Ok(licensePlateDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing GET in {nameof(GetLicensePlates)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }
        
        /// <summary>
        /// Gets a license plate by ID.
        /// </summary>
        /// <param name="id">The ID of the license plate.</param>
        /// <returns>The license plate details.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<LicensePlateReadOnlyDto>> GetLicensePlate(int id)
        {
            try
            {
                var licensePlate = await _licensePlateRepository.GetAsync(id);

                if (licensePlate == null)
                {
                    _logger.LogWarning($"Record Not Found: {nameof(GetLicensePlate)} - ID: {id}");
                    return NotFound();
                }

                var licensePlateDto = _mapper.Map<LicensePlateReadOnlyDto>(licensePlate);
                return Ok(licensePlateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing GET in {nameof(GetLicensePlates)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }
        
        /// <summary>
        /// Updates a license plate.
        /// </summary>
        /// <param name="id">The ID of the license plate.</param>
        /// <param name="licensePlateDto">The license plate data transfer object.</param>
        /// <returns>No content.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PutLicensePlate(int id, LicensePlateUpdateDto licensePlateDto)
        {
            if (id != licensePlateDto.Id)
            {
                _logger.LogWarning($"Update ID invalid in {nameof(PutLicensePlate)} - ID: {id}");
                return BadRequest();
            }

            var licensePlate = await _licensePlateRepository.GetAsync(id);

            if (licensePlate == null)
            {
                _logger.LogWarning($"{nameof(LicensePlate)} record not found in {nameof(PutLicensePlate)} - ID: {id}");
                return NotFound();
            }

            _mapper.Map(licensePlateDto, licensePlate);
            
            try
            {
                await _licensePlateRepository.UpdateAsync(licensePlate);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await LicensePlateExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, $"Error Performing GET in {nameof(PutLicensePlate)}");
                    return StatusCode(500, Messages.Error500Message);
                }
            }

            return NoContent();
        }
        
        /// <summary>
        /// Creates a new license plate.
        /// </summary>
        /// <param name="licensePlateDto">The license plate data transfer object.</param>
        /// <returns>The created license plate.</returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<LicensePlateCreateDto>> PostLicensePlate(LicensePlateCreateDto licensePlateDto)
        {
            try
            {
                var licensePlate = _mapper.Map<LicensePlate>(licensePlateDto);
                await _licensePlateRepository.AddAsync(licensePlate);

                return CreatedAtAction(nameof(GetLicensePlate), new { id = licensePlate.Id }, licensePlate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing POST in {nameof(PostLicensePlate)}", licensePlateDto);
                return StatusCode(500, Messages.Error500Message);
            }
            
        }
        
        /// <summary>
        /// Deletes a license plate by ID.
        /// </summary>
        /// <param name="id">The ID of the license plate.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteLicensePlate(int id)
        {
            try
            {
                var licensePlate = await _licensePlateRepository.GetAsync(id);
                if (licensePlate == null)
                {
                    _logger.LogWarning($"{nameof(LicensePlate)} record not found in {nameof(DeleteLicensePlate)} - ID: {id}");
                    return NotFound();
                }

                await _licensePlateRepository.DeleteAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing DELETE in {nameof(DeleteLicensePlate)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        /// <summary>
        /// Checks if a license plate exists by ID.
        /// </summary>
        /// <param name="id">The ID of the license plate.</param>
        /// <returns>True if the license plate exists; otherwise, false.</returns>
        private async Task<bool> LicensePlateExists(int id)
        {
            return await _licensePlateRepository.Exists(id);
        }
    }
}
