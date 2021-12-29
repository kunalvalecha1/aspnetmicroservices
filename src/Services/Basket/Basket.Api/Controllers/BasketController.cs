using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Basket.Api.Entities;
using Basket.Api.GrpcServices;
using Basket.Api.Repositories;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Basket.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly ILogger<BasketController> _logger;
        private readonly DiscountGrpcService _discountGrpcService;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        public BasketController(IBasketRepository repository, DiscountGrpcService discountGrpcService, ILogger<BasketController> logger, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _repository = repository;
            _logger = logger;
            _discountGrpcService = discountGrpcService;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            var basket = await _repository.GetBasket(userName);

            return Ok(basket ?? new ShoppingCart(userName));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            //TODO: Connect Discount.Grpc for getting discount
            foreach(var item in basket.Items)
            {
              var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
                item.Price -= coupon.Amount;
            }
            return Ok(await _repository.UpdateBasket(basket));
            //return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
        }


        [HttpDelete("{userName}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            await _repository.DeleteBasket(userName);
            return Ok();
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType( (int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            // get existing basket with total price
            var basket = await _repository.GetBasket(basketCheckout.UserName);
            if(basket == null)
            {
                return BadRequest();
            }

            // create basketcheckout event -- set TotalPrice in baskecheckout eventmessage
            var eventMsg = _mapper.Map<BasketCheckoutEvent>(basketCheckout);

            //send checkout event to rabbitmq
            eventMsg.TotalPrice = basket.TotalPrice;
            await _publishEndpoint.Publish(eventMsg);

            //remove to basket
            await _repository.DeleteBasket(basketCheckout.UserName);

            return Accepted();
        }
    }
}