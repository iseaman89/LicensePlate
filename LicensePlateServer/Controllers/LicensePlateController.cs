using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LicensePlateDataShared.Models;
using LicensePlateDataShared.Static;
using LicensePlateServer.Data;
using LicensePlateServer.Models.LicensePlates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LicensePlateServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LicensePlateController : ControllerBase
    {
        private readonly LicensePlateDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<LicensePlateController> _logger;

        public LicensePlateController(LicensePlateDbContext context, IMapper mapper, ILogger<LicensePlateController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/Authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LicensePlateReadOnlyDto>>> GetLicensePlates()
        {
            try
            {
                var licensePlates = await _context.LicensePlates.ToListAsync();
                var licensePlateDtos = _mapper.Map<IEnumerable<LicensePlateReadOnlyDto>>(licensePlates);
                return Ok(licensePlateDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing GET in {nameof(GetLicensePlates)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // GET: api/Authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LicensePlateReadOnlyDto>> GetLicensePlate(int id)
        {
            try
            {
                var licensePlate = await _context.LicensePlates.FindAsync(id);

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

        // PUT: api/Authors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PutLicensePlate(int id, LicensePlateUpdateDto licensePlateDto)
        {
            if (id != licensePlateDto.Id)
            {
                _logger.LogWarning($"Update ID invalid in {nameof(PutLicensePlate)} - ID: {id}");
                return BadRequest();
            }

            var licensePlate = await _context.LicensePlates.FindAsync(id);

            if (licensePlate == null)
            {
                _logger.LogWarning($"{nameof(LicensePlate)} record not found in {nameof(PutLicensePlate)} - ID: {id}");
                return NotFound();
            }

            _mapper.Map(licensePlateDto, licensePlate);
            _context.Entry(licensePlate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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

        // POST: api/Authors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<LicensePlateCreateDto>> PostLicensePlate(LicensePlateCreateDto licensePlateDto)
        {
            try
            {
                var licensePlate = _mapper.Map<LicensePlate>(licensePlateDto);
                await _context.LicensePlates.AddAsync(licensePlate);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetLicensePlate), new { id = licensePlate.Id }, licensePlate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing POST in {nameof(PostLicensePlate)}", licensePlateDto);
                return StatusCode(500, Messages.Error500Message);
            }
            
        }

        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteLicensePlate(int id)
        {
            try
            {
                var licensePlate = await _context.LicensePlates.FindAsync(id);
                if (licensePlate == null)
                {
                    _logger.LogWarning($"{nameof(LicensePlate)} record not found in {nameof(DeleteLicensePlate)} - ID: {id}");
                    return NotFound();
                }

                _context.LicensePlates.Remove(licensePlate);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing DELETE in {nameof(DeleteLicensePlate)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        private async Task<bool> LicensePlateExists(int id)
        {
            return await _context.LicensePlates.AnyAsync(e => e.Id == id);
        }
    }
}
