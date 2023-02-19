using AutoMapper;
using Store.ClassDomain.Domains;
using Store.InterfaceService.InterfacesBase;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Store.ClassDTO.DTOs.Request.Products;
using Store.ClassDTO.DTOs.Response.Products;
using Store.Common.Exceptions;
using Store.ClassDTO.Mapping;
using Store.ClassDTO.DTOs;

namespace Store.WebApi.Controllers.Api.Admin
{
    [EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private IUnitOfWorkStoreService _UnitOfWorkStoreService; IMapper _IMapperProducts;
        public ProductsController(IUnitOfWorkStoreService UnitOfWorkStoreService, IConfiguration Configuration) { _UnitOfWorkStoreService = UnitOfWorkStoreService; _IMapperProducts = MapperProducts.MapTo(); }

        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("GetAll")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var Products = await _UnitOfWorkStoreService._IProductsService.GetAll();
            var _Products = Products.Select(P => _IMapperProducts.Map<Products, DtoProducts>(P)).ToList();
            return Ok(new ResponseGetAllProducts { Products = _Products, CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });
        }


        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("GetAll")]
        [HttpPost]
        public async Task<IActionResult> GetAll([FromBody] RequestGetAllProducts BodyProducts)
        {
            var Products = await _UnitOfWorkStoreService._IProductsService.GetAll(P => P.Products_GroupProductsID == BodyProducts.GPrdc_ID && P.Products_StoreID == BodyProducts.Str_ID);
            var _Products = Products.Select(P => _IMapperProducts.Map<Products, DtoProducts>(P)).ToList();
            return Ok(new ResponseGetAllProducts { Products = _Products, CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });
        }


        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("GetByID")]
        [HttpPost]
        public async Task<IActionResult> GetByID([FromBody] RequestGetByIDProducts BodyProducts)
        {
            if (BodyProducts == null) return Ok(new ResponseGetByIDProducts { Products = null, CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestNull });
            var Product = await _UnitOfWorkStoreService._IProductsService.GetByWhere(P => P.Products_ID == BodyProducts.Prdc_ID);
            var _Product = _IMapperProducts.Map<Products, DtoProducts>(Product);
            return Ok(new ResponseGetByIDProducts { Products = _Product, CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });
        }

        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("Insert")]
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] RequestInsertProducts BodyProducts)
        {
            if (BodyProducts == null) return Ok(new ResponseInsertProducts { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestNull });
            var _Product = _IMapperProducts.Map<DtoProducts, Products>(BodyProducts);
            await _UnitOfWorkStoreService._IProductsService.Insert(_Product);
            if (await _UnitOfWorkStoreService.SaveChange_DataBase_Async() > 0) return Ok(new ResponseInsertProducts { CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });
            return Ok(new ResponseInsertProducts { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.Error });
        }

        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] RequestUpdateProducts BodyProducts)
        {
            if (BodyProducts == null) return Ok(new ResponseUpdateProducts { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestNull });
            var _Product = await _UnitOfWorkStoreService._IProductsService.GetByWhere(P => P.Products_ID == BodyProducts.Prdc_ID);
            if (_Product == null) return Ok(new ResponseUpdateProducts { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestNull });
            _Product.Products_Price = BodyProducts.Prdc_Pric;
            _Product.Products_Name = BodyProducts.Prdc_Name;
            _Product.Products_Count = BodyProducts.Prdc_Cunt;
            _UnitOfWorkStoreService._IProductsService.Update(_Product);
            if (await _UnitOfWorkStoreService.SaveChange_DataBase_Async() > 0) return Ok(new ResponseUpdateProducts { CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });
            return Ok(new ResponseUpdateProducts { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestFailt });
        }

        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("Delete")]
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] RequestDeleteProducts BodyProducts)
        {
            if (BodyProducts == null) return Ok(new ResponseDeleteProducts { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestNull });
            var _Product = await _UnitOfWorkStoreService._IProductsService.GetByWhere(P => P.Products_ID == BodyProducts.Prdc_ID);
            if (_Product == null) return Ok(new ResponseDeleteProducts { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestNull });
            _UnitOfWorkStoreService._IProductsService.Delete(_Product);
            if (await _UnitOfWorkStoreService.SaveChange_DataBase_Async() > 0) return Ok(new ResponseDeleteProducts { CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });
            return Ok(new ResponseDeleteProducts { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestFailt });
        }
    }
}
