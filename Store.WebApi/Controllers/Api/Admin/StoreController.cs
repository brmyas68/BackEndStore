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


namespace Store.WebApi.Controllers.Api.Admin
{
    [EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private IUnitOfWorkStoreService _UnitOfWorkStoreService; IMapper _IMapperStore;
        public StoreController(IUnitOfWorkStoreService UnitOfWorkStoreService, IConfiguration Configuration) { _UnitOfWorkStoreService = UnitOfWorkStoreService; _IMapperStore = MapperStore.MapTo(); }

        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("GetAll")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var Stores = await _UnitOfWorkStoreService._IStoreService.GetAll();
            var _Stores = Stores.Select(S => _IMapperStore.Map<ClassDomain.Domains.Store, DtoStore>(S)).ToList();
            return Ok(new ResponseGetAllStore { Stores = _Stores, CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });
        }


        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("GetByID")]
        [HttpPost]
        public async Task<IActionResult> GetByID([FromBody] RequestGetByIDStore BodyStore)
        {
            if (BodyStore == null) return Ok(new ResponseGetByIDStore { Store = null, CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestNull });
            var Store = await _UnitOfWorkStoreService._IStoreService.GetByWhere(S => S.Store_ID == BodyStore.Str_ID);
            var _Store = _IMapperStore.Map<ClassDomain.Domains.Store, DtoStore>(Store);
            return Ok(new ResponseGetByIDStore { Store = _Store, CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });
        }

        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("Insert")]
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] RequestInsertStore BodyStore)
        {
            if (BodyStore == null) return Ok(new ResponseInsertStore { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestNull });
            var _Store = _IMapperStore.Map<DtoStore, ClassDomain.Domains.Store>(BodyStore);
            await _UnitOfWorkStoreService._IStoreService.Insert(_Store);
            if (await _UnitOfWorkStoreService.SaveChange_DataBase_Async() > 0) return Ok(new ResponseInsertStore { CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });
            return Ok(new ResponseInsertStore { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.Error });
        }

        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] RequestUpdateStore BodyStore)
        {
            if (BodyStore == null) return Ok(new ResponseUpdateStore { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestNull });
            var _Store = await _UnitOfWorkStoreService._IStoreService.GetByWhere(S => S.Store_ID == BodyStore.Str_ID);
            if (_Store == null) return Ok(new ResponseUpdateStore { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestNull });
            _Store.Store_Address = BodyStore.Str_Adrs;
            _Store.Store_Name = BodyStore.Str_Name;
            _Store.Store_Tell = BodyStore.Str_Tel;
            _UnitOfWorkStoreService._IStoreService.Update(_Store);
            if (await _UnitOfWorkStoreService.SaveChange_DataBase_Async() > 0) return Ok(new ResponseUpdateStore { CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });
            return Ok(new ResponseUpdateStore { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestFailt });
        }

        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("Delete")]
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] RequestDeleteStore BodyStore)
        {
            if (BodyStore == null) return Ok(new ResponseDeleteStore { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestNull });
            var _Store = await _UnitOfWorkStoreService._IStoreService.GetByWhere(S => S.Store_ID == BodyStore.Str_ID);
            if (_Store == null) return Ok(new ResponseDeleteStore { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestNull });
            _UnitOfWorkStoreService._IStoreService.Delete(_Store);
            if (await _UnitOfWorkStoreService.SaveChange_DataBase_Async() > 0) return Ok(new ResponseDeleteStore { CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });
            return Ok(new ResponseDeleteStore { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestFailt });
        }
    }
}
