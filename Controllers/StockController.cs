using finshark.Data;
using finshark.Mappers;
using finshark.Dtos.Stock;
using Microsoft.AspNetCore.Mvc;
using finshark.Interfaces;
using finshark.Helpers;

namespace finshark.Controllers
{
    [Route("api/stocks")]
    [ApiController]
    
    public class StockController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IStockRepository _stockRepository;
        public StockController(ApplicationDBContext dbContext, IStockRepository stockRepository)
        {
            _dbContext = dbContext;
            _stockRepository = stockRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var stocks = await _stockRepository.GetAllAsync(query);
            var stockDto = stocks.Select(s => s.ToStockDto());

            return Ok(stockDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var stock = await _stockRepository.GetByIdAsync(id);

            if (stock == null)
            {
                return NotFound();
            }

            else
            {
                return Ok(stock.ToStockDto());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockReqDto stockDto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var stockModel = stockDto.ToStockFromCreateDto();
            await _stockRepository.CreateAsync(stockModel);

            return CreatedAtAction(nameof(GetById), new { id = stockModel.Id }, stockModel.ToStockDto());
        }


        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockReqDto updateDto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var stockModel = await _stockRepository.UpdateAsync(id, updateDto);

            if(stockModel == null)
            {
                return NotFound();
            }

            return Ok(stockModel.ToStockDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var stockModel = await _stockRepository.DeleteAsync(id);

            if(stockModel == null)
            {
                return NotFound();
            }
            
            return NoContent();
        }
    }
}