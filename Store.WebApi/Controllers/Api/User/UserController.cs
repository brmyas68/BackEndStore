
using AutoMapper;
using Store.ClassDomain.Domains;
using Store.InterfaceService.InterfacesBase;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Store.ClassDTO.DTOs.Request.Store;
using Store.ClassDTO.DTOs.Response.Store;
using Store.Common.Exceptions;
using Store.ClassDTO.Mapping;
using Store.ClassDTO.DTOs;
using Store.ClassDTO.DTOs.Response.GroupProducts;
using Store.Service.ProtoBuffer.UC;
using Store.ClassDTO.DTOs.Response.User;
using Grpc.Core;
using Store.ClassDTO.DTOs.Response.Products;
using Store.ClassDTO.DTOs.Request.Products;

namespace Store.WebApi.Controllers.Api.User
{
    [EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUnitOfWorkStoreService _UnitOfWorkStoreService; IMapper _IMapperGroupProducts; IMapper _IMapperProducts;
        public UserController(IUnitOfWorkStoreService UnitOfWorkStoreService, IConfiguration Configuration) { _UnitOfWorkStoreService = UnitOfWorkStoreService; _IMapperGroupProducts = MapperGroupProducts.MapTo(); _IMapperProducts = MapperProducts.MapTo(); }



        [Route("VitrinProduct")]
        [HttpPost]
        public async Task<IActionResult> VitrinProducts([FromBody] RequestVitrinCount BodyCount)
        {
            var Products = await _UnitOfWorkStoreService._IProductsService.LastVitrinProduct(BodyCount.Count);
            var _Products = Products.Select(P => _IMapperProducts.Map<Products, DtoProducts>(P)).ToList();
            return Ok(new ResponseVitrinProducts { Products = _Products, CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });

        }

        [Route("GroupsProduct")]
        [HttpGet]
        public async Task<IActionResult> GroupsProduct()
        {
            var GroupProducts = await _UnitOfWorkStoreService._IGroupProductsService.GetAll();
            var _GroupProducts = GroupProducts.Select(GP => _IMapperGroupProducts.Map<ClassDomain.Domains.GroupProducts, DtoGroupProducts>(GP)).ToList();
            return Ok(new ResponseGetAllGroupProducts { GroupProducts = _GroupProducts, CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });

        }

        [Route("ProductDetail")]
        [HttpPost]
        public async Task<IActionResult> ProductDetail([FromBody] RequestGetByIDProducts BodyProducts)
        {
            if (BodyProducts == null) return Ok(new ResponseGetByIDProducts { Products = null, CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestNull });
            var Product = await _UnitOfWorkStoreService._IProductsService.GetByWhere(P => P.Products_ID == BodyProducts.Prdc_ID);
            var _Product = _IMapperProducts.Map<Products, DtoProducts>(Product);
            return Ok(new ResponseGetByIDProducts { Products = _Product, CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });

        }

        //[Route("Orders")]
        //[HttpPost]
        //public async Task<IActionResult> Orders([FromBody] int UserID)
        //{
        //    var Orders = await _UnitOfWorkStoreService._IOrdersService.GetAll();
        //    var _Orders = Orders.Select(O => _IMapperOrders.Map<Orders, DtoOrders>(O)).ToList();
        //    return Ok(new ResponseGetAllOrders { Orders = _Orders, CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });

        //}

        //[Route("OrdersProduct")]
        //[HttpPost]
        //public async Task<IActionResult> OrdersProduct([FromBody] int orderID)
        //{
        //    return Ok("");
        //}

        //[Route("InsertOrderBuy")]
        //[HttpPost]
        //public async Task<IActionResult> InsertOrderBuy([FromBody] int Count)
        //{
        //    return Ok("");
        //}



        //[Route("CommentsUser")]
        //[HttpPost]
        //public async Task<IActionResult> CommentsUser([FromBody] int userID)
        //{
        //    return Ok("");
        //}

        //[Route("CommentsProduct")]
        //[HttpPost]
        //public async Task<IActionResult> CommentsProduct([FromBody] int PID)
        //{
        //    return Ok("");
        //}

        //[Route("InsertComment")]
        //[HttpPost]
        //public async Task<IActionResult> InsertComment([FromBody] int Count)
        //{
        //    return Ok("");
        //}



        //[Route("UpdateProfile")]
        //[HttpPost]
        //public async Task<IActionResult> UpdateProfile([FromBody] int Count)
        //{
        //    return Ok("");
        //}



        //[Route("Accounts")]
        //[HttpPost]
        //public async Task<IActionResult> Accounts([FromBody] int PID, int UserID)
        //{
        //    return Ok("");
        //}


    }
}
