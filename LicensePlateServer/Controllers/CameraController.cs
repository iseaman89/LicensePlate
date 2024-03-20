using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LicensePlateDataShared.Models;
using LicensePlateDataShared.Static;
using LicensePlateServer.Data;
using LicensePlateServer.Models.Cameras;
using Microsoft.AspNetCore.Authorization;

namespace LicensePlateServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CameraController : ControllerBase
    {
        private readonly LicensePlateDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CameraController> _logger;

        public CameraController(LicensePlateDbContext context, IMapper mapper, ILogger<CameraController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/Authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CameraReadOnlyDto>>> GetCameras()
        {
            try
            {
                var cameras = await _context.Cameras.ToListAsync();
                var cameraDtos = _mapper.Map<IEnumerable<CameraReadOnlyDto>>(cameras);
                return Ok(cameraDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing GET in {nameof(GetCameras)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // GET: api/Authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CameraReadOnlyDto>> GetCamera(int id)
        {
            try
            {
                var camera = await _context.Cameras.FindAsync(id);

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

        // PUT: api/Authors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PutCamera(int id, CameraUpdateDto cameraDto)
        {
            if (id != cameraDto.Id)
            {
                _logger.LogWarning($"Update ID invalid in {nameof(PutCamera)} - ID: {id}");
                return BadRequest();
            }

            var camera = await _context.Cameras.FindAsync(id);

            if (camera == null)
            {
                _logger.LogWarning($"{nameof(Camera)} record not found in {nameof(PutCamera)} - ID: {id}");
                return NotFound();
            }

            _mapper.Map(cameraDto, camera);
            _context.Entry(camera).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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

        // POST: api/Authors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<CameraCreateDto>> PostCamera(CameraCreateDto cameraDto)
        {
            try
            {
                var camera = _mapper.Map<Camera>(cameraDto);
                await _context.Cameras.AddAsync(camera);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCamera), new { id = camera.Id }, camera);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing POST in {nameof(PostCamera)}", cameraDto);
                return StatusCode(500, Messages.Error500Message);
            }
            
        }

        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteCamera(int id)
        {
            try
            {
                var camera = await _context.Cameras.FindAsync(id);
                if (camera == null)
                {
                    _logger.LogWarning($"{nameof(Camera)} record not found in {nameof(DeleteCamera)} - ID: {id}");
                    return NotFound();
                }

                _context.Cameras.Remove(camera);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing DELETE in {nameof(DeleteCamera)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        private async Task<bool> CameraExists(int id)
        {
            return await _context.Cameras.AnyAsync(c => c.Id == id);
        }
    }
}
