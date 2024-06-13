using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LicensePlateDataShared.Models;
using LicensePlateDataShared.Static;
using LicensePlateServer.Models.Cameras;
using LicensePlateServer.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace LicensePlateServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CameraController : ControllerBase
    {
        private readonly ICameraRepository _cameraRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CameraController> _logger;

        public CameraController(ICameraRepository cameraRepository, IMapper mapper, ILogger<CameraController> logger)
        {
            _cameraRepository = cameraRepository;
            _mapper = mapper;
            _logger = logger;
        }
        
        /// <summary>
        /// Gets all cameras.
        /// </summary>
        /// <returns>A list of cameras.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CameraReadOnlyDto>>> GetCameras()
        {
            try
            {
                var cameras = await _cameraRepository.GetAllAsync();
                var cameraDtos = _mapper.Map<IEnumerable<CameraReadOnlyDto>>(cameras);
                return Ok(cameraDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing GET in {nameof(GetCameras)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }
        
        /// <summary>
        /// Gets a camera by ID.
        /// </summary>
        /// <param name="id">The ID of the camera.</param>
        /// <returns>The camera details.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CameraReadOnlyDto>> GetCamera(int id)
        {
            try
            {
                var camera = await _cameraRepository.GetAsync(id);

                if (camera == null)
                {
                    _logger.LogWarning($"Record Not Found: {nameof(GetCamera)} - ID: {id}");
                    return NotFound();
                }

                var cameraDto = _mapper.Map<CameraReadOnlyDto>(camera);
                return Ok(cameraDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing GET in {nameof(GetCameras)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }
        
        /// <summary>
        /// Updates a camera.
        /// </summary>
        /// <param name="id">The ID of the camera.</param>
        /// <param name="cameraDto">The camera data transfer object.</param>
        /// <returns>No content.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PutCamera(int id, CameraUpdateDto cameraDto)
        {
            if (id != cameraDto.Id)
            {
                _logger.LogWarning($"Update ID invalid in {nameof(PutCamera)} - ID: {id}");
                return BadRequest();
            }

            var camera = await _cameraRepository.GetAsync(id);

            if (camera == null)
            {
                _logger.LogWarning($"{nameof(Camera)} record not found in {nameof(PutCamera)} - ID: {id}");
                return NotFound();
            }

            _mapper.Map(cameraDto, camera);

            try
            {
                await _cameraRepository.UpdateAsync(camera);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await CameraExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, $"Error Performing GET in {nameof(PutCamera)}");
                    return StatusCode(500, Messages.Error500Message);
                }
            }

            return NoContent();
        }
        
        /// <summary>
        /// Creates a new camera.
        /// </summary>
        /// <param name="cameraDto">The camera data transfer object.</param>
        /// <returns>The created camera.</returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<CameraCreateDto>> PostCamera(CameraCreateDto cameraDto)
        {
            try
            {
                var camera = _mapper.Map<Camera>(cameraDto);
                await _cameraRepository.AddAsync(camera);

                return CreatedAtAction(nameof(GetCamera), new { id = camera.Id }, camera);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing POST in {nameof(PostCamera)}", cameraDto);
                return StatusCode(500, Messages.Error500Message);
            }
            
        }
        
        /// <summary>
        /// Deletes a camera by ID.
        /// </summary>
        /// <param name="id">The ID of the camera.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteCamera(int id)
        {
            try
            {
                var camera = await _cameraRepository.GetAsync(id);
                if (camera == null)
                {
                    _logger.LogWarning($"{nameof(Camera)} record not found in {nameof(DeleteCamera)} - ID: {id}");
                    return NotFound();
                }

                await _cameraRepository.DeleteAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing DELETE in {nameof(DeleteCamera)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        /// <summary>
        /// Checks if a camera exists by ID.
        /// </summary>
        /// <param name="id">The ID of the camera.</param>
        /// <returns>True if the camera exists; otherwise, false.</returns>
        private async Task<bool> CameraExists(int id)
        {
            return await _cameraRepository.Exists(id);
        }
    }
}
