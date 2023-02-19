
using AutoMapper;

using Store.InterfaceService.InterfacesBase;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

using Store.Common.Exceptions;
using Store.ClassDTO.Mapping;
using Store.ClassDTO.DTOs;
using Store.ClassDTO.DTOs.Response.GroupProducts;
using Store.ClassDTO.DTOs.Request.GroupProducts;

namespace Store.WebApi.Controllers.Api.Admin
{
    [EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class GroupProductsController : ControllerBase
    {
        private IUnitOfWorkStoreService _UnitOfWorkStoreService; IMapper _IMapperGroupProducts;
        public GroupProductsController(IUnitOfWorkStoreService UnitOfWorkStoreService, IConfiguration Configuration) { _UnitOfWorkStoreService = UnitOfWorkStoreService; _IMapperGroupProducts = MapperGroupProducts.MapTo(); }

        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("GetAll")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var GroupProducts = await _UnitOfWorkStoreService._IGroupProductsService.GetAll();
            var _GroupProducts = GroupProducts.Select(GP => _IMapperGroupProducts.Map<ClassDomain.Domains.GroupProducts, DtoGroupProducts>(GP)).ToList();
            return Ok(new ResponseGetAllGroupProducts { GroupProducts = _GroupProducts, CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });
        }


        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("GetByID")]
        [HttpPost]
        public async Task<IActionResult> GetByID([FromBody] RequestGetByIDGroupProducts BodyGroupProduct)
        {
            if (BodyGroupProduct == null) return Ok(new ResponseGetByIDGroupProducts { GroupProduct = null, CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestNull });
            var GroupProduct = await _UnitOfWorkStoreService._IGroupProductsService.GetByWhere(GP => GP.GroupProducts_ID == BodyGroupProduct.GPrdc_ID);
            var _GroupProduct = _IMapperGroupProducts.Map<ClassDomain.Domains.GroupProducts, DtoGroupProducts>(GroupProduct);
            return Ok(new ResponseGetByIDGroupProducts { GroupProduct = _GroupProduct, CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });
        }

        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("Insert")]
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] RequestInsertGroupProduct BodyGroupProduct)
        {
            if (BodyGroupProduct == null) return Ok(new ResponseInsertGroupProduct { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestNull });
            var _GroupProduct = _IMapperGroupProducts.Map<DtoGroupProducts, ClassDomain.Domains.GroupProducts>(BodyGroupProduct);
            await _UnitOfWorkStoreService._IGroupProductsService.Insert(_GroupProduct);
            if (await _UnitOfWorkStoreService.SaveChange_DataBase_Async() > 0) return Ok(new ResponseInsertGroupProduct { CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });
            return Ok(new ResponseInsertGroupProduct { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.Error });
        }

        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] RequestUpdateGroupProduct BodyGroupProduct)
        {
            if (BodyGroupProduct == null) return Ok(new ResponseUpdateGroupProduct { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestNull });
            var _GroupProduct = await _UnitOfWorkStoreService._IGroupProductsService.GetByWhere(GP => GP.GroupProducts_ID == BodyGroupProduct.GPrdc_ID);
            if (_GroupProduct == null) return Ok(new ResponseUpdateGroupProduct { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestNull });
            _GroupProduct.GroupProducts_Name = BodyGroupProduct.GPrdc_Name;
            _UnitOfWorkStoreService._IGroupProductsService.Update(_GroupProduct);
            if (await _UnitOfWorkStoreService.SaveChange_DataBase_Async() > 0) return Ok(new ResponseUpdateGroupProduct { CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });
            return Ok(new ResponseUpdateGroupProduct { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestFailt });
        }

        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("Delete")]
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] RequestDeleteGroupProduct BodyGroupProduct)
        {
            if (BodyGroupProduct == null) return Ok(new ResponseDeleteGroupProduct { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestNull });
            var _GroupProduct = await _UnitOfWorkStoreService._IGroupProductsService.GetByWhere(GP => GP.GroupProducts_ID == BodyGroupProduct.GPrdc_ID);
            if (_GroupProduct == null) return Ok(new ResponseDeleteGroupProduct { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestNull });
            _UnitOfWorkStoreService._IGroupProductsService.Delete(_GroupProduct);
            if (await _UnitOfWorkStoreService.SaveChange_DataBase_Async() > 0) return Ok(new ResponseDeleteGroupProduct { CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });
            return Ok(new ResponseDeleteGroupProduct { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestFailt });
        }
    }
}
