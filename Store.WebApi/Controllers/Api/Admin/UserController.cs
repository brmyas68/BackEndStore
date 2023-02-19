
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

namespace Store.WebApi.Controllers.Api.Admin
{
    [EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUnitOfWorkStoreService _UnitOfWorkStoreService; IMapper _IMapperGroupProducts;
        public UserController(IUnitOfWorkStoreService UnitOfWorkStoreService, IConfiguration Configuration) { _UnitOfWorkStoreService = UnitOfWorkStoreService; _IMapperGroupProducts = MapperGroupProducts.MapTo(); }

        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("GetAll")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {


            var _ResponseGetAllUsers = new ResponseGetAllUsers();

            try
            {
                var ClientUC = UcClient.GetChannel("http://127.0.0.1", 8080);
                using (var ResponseGetAllUser = ClientUC.GetAllUser(new RequestEmpty() { }))
                {
                    var _ResponseGetAllUser = ResponseGetAllUser.ResponseStream;
                    while (await _ResponseGetAllUser.MoveNext())
                    {
                        _ResponseGetAllUsers.Users.Add(
                            new DtoUser()
                            {
                                User_ID = _ResponseGetAllUser.Current.User.UserID,
                                User_Fname = _ResponseGetAllUser.Current.User.UserFname,
                                User_Lname = _ResponseGetAllUser.Current.User.UserLname,
                                User_Mobile = _ResponseGetAllUser.Current.User.UserMobile,
                                User_Address = _ResponseGetAllUser.Current.User.UserAddress,
                                User_CodeMeli = _ResponseGetAllUser.Current.User.UserCodeMeli,
                                User_DateTime = _ResponseGetAllUser.Current.User.UserDateTime.ToDateTime(),
                                User_IsActive = _ResponseGetAllUser.Current.User.UserIsActive,
                            });
                    }
                    _ResponseGetAllUsers.MessageStatus = MessageException.MessagesStatus.Success;
                    _ResponseGetAllUsers.CodeStatus = MessageException.CodeStatus.Status200;

                }
            }
            catch (Exception)
            {
                _ResponseGetAllUsers.Users = null;
                _ResponseGetAllUsers.MessageStatus = MessageException.MessagesStatus.RequestFailt;
                _ResponseGetAllUsers.CodeStatus = MessageException.CodeStatus.Status400;
            }


            return Ok(_ResponseGetAllUsers);

        }
    }


}

