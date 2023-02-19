using AutoMapper;
using Store.ClassDomain.Domains;
using Store.InterfaceService.InterfacesBase;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Store.ClassDTO.DTOs.Request.Orders;
using Store.ClassDTO.DTOs.Response.Orders;
using Store.Common.Exceptions;
using Store.ClassDTO.Mapping;
using Store.ClassDTO.DTOs;

namespace Store.WebApi.Controllers.Api.Admin
{
    [EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private IUnitOfWorkStoreService _UnitOfWorkStoreService; IMapper _IMapperOrders;
        public OrdersController(IUnitOfWorkStoreService UnitOfWorkStoreService, IConfiguration Configuration) { _UnitOfWorkStoreService = UnitOfWorkStoreService; _IMapperOrders = MapperOrders.MapTo(); }

        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("GetAll")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var Orders = await _UnitOfWorkStoreService._IOrdersService.GetAll();
            var _Orders = Orders.Select(O => _IMapperOrders.Map<Orders, DtoOrders>(O)).ToList();
            return Ok(new ResponseGetAllOrders { Orders = _Orders, CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });
        }


        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("GetByID")]
        [HttpPost]
        public async Task<IActionResult> GetByID([FromBody] RequestGetByIDOrders BodyOrder)
        {
            if (BodyOrder == null) return Ok(new ResponseGetByIDOrders { Orders = null, CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestNull });
            var Order = await _UnitOfWorkStoreService._IOrdersService.GetByWhere(O => O.Orders_ID == BodyOrder.Ord_ID);
            var _Order = _IMapperOrders.Map<Orders, DtoOrders>(Order);
            return Ok(new ResponseGetByIDOrders { Orders = _Order, CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });
        }

        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("Insert")]
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] RequestInsertOrders BodyOrder)
        {
            if (BodyOrder == null) return Ok(new ResponseInsertOrders { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestNull });
            var _Order = _IMapperOrders.Map<DtoOrders, Orders>(BodyOrder);
            await _UnitOfWorkStoreService._IOrdersService.Insert(_Order);
            if (await _UnitOfWorkStoreService.SaveChange_DataBase_Async() > 0) return Ok(new ResponseInsertOrders { CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });
            return Ok(new ResponseInsertOrders { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.Error });
        }

        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] RequestUpdateOrders BodyOrder)
        {
            if (BodyOrder == null) return Ok(new ResponseUpdateOrders { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestNull });
            var _Order = await _UnitOfWorkStoreService._IOrdersService.GetByWhere(O => O.Orders_ID == BodyOrder.Ord_ID);
            if (_Order == null) return Ok(new ResponseUpdateOrders { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestNull });
            _Order.Orders_ProductsPrice = BodyOrder.Ord_PrdcPric;
            _Order.Orders_ProductsCount = BodyOrder.Ord_PrdcCunt;
            _UnitOfWorkStoreService._IOrdersService.Update(_Order);
            if (await _UnitOfWorkStoreService.SaveChange_DataBase_Async() > 0) return Ok(new ResponseUpdateOrders { CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });
            return Ok(new ResponseUpdateOrders { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestFailt });
        }

        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("Delete")]
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] RequestDeleteOrders BodyOrder)
        {
            if (BodyOrder == null) return Ok(new ResponseDeleteOrders { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestNull });
            var _Order = await _UnitOfWorkStoreService._IOrdersService.GetByWhere(O => O.Orders_ID == BodyOrder.Ord_ID);
            if (_Order == null) return Ok(new ResponseDeleteOrders { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestNull });
            _UnitOfWorkStoreService._IOrdersService.Delete(_Order);
            if (await _UnitOfWorkStoreService.SaveChange_DataBase_Async() > 0) return Ok(new ResponseDeleteOrders { CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });
            return Ok(new ResponseDeleteOrders { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestFailt });
        }





    }
}
