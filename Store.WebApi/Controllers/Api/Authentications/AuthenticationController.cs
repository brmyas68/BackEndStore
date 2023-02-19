
using Store.InterfaceService.InterfacesBase;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

using Store.ClassDTO.DTOs.Response.Store;


using Microsoft.AspNetCore.Authorization;
using Store.Service.ProtoBuffer.UC;
using Store.ClassDTO.DTOs.Request.Authorize;
using Store.Common.Exceptions;
using Store.ClassDTO.DTOs.Response.Authorize;
using Grpc.Core;
using Store.Common.Authorization;
using Store.ClassDTO.DTOs.Response.User;
using Store.ClassDTO.DTOs;
using Store.ClassDTO.DTOs.Request.User;

namespace Store.WebApi.Controllers.Api.Authentications
{
    [EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private IUnitOfWorkStoreService _UnitOfWorkStoreService;
        public AuthenticationController(IUnitOfWorkStoreService UnitOfWorkStoreService, IConfiguration Configuration) { _UnitOfWorkStoreService = UnitOfWorkStoreService; }


        [AllowAnonymous]
        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] RequestAuthorizeLogin BodyLogin)
        {
            try
            {
                var ClientUC = UcClient.GetChannel("http://127.0.0.1", 8080);
                var ResponseLogin = await ClientUC.LoginAsync(new RequestLogin() { Mobile = BodyLogin.Mobile });
                if (ResponseLogin.Status.StatusCode == global::StatusCode.Status200 && ResponseLogin.Status.StatusMessage == global::StatusMessage.Success)
                {
                    return Ok(new ResponseAuthorizeLogin { Token = ResponseLogin.Token, CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });
                }
                return Ok(new ResponseAuthorizeLogin { Token = "", CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestFailt });
            }
            catch (Exception)
            {
                return Ok(new ResponseAuthorizeLogin { Token = "", CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.Error });
            }
        }


        [AllowAnonymous]
        [Route("LogOut")]
        [HttpPost]
        public async Task<IActionResult> LogOut([FromBody] RequestAuthorizeLogOut BodyLogOut)
        {
            try
            {
                var ClientUC = UcClient.GetChannel("http://127.0.0.1", 8080);
                var _Token = AuthenticationHttpContext.GetToken(this.HttpContext);
                Metadata MData = new Metadata();
                MData.Add("authorization", _Token);
                var ResponseLogOut = await ClientUC.LogOutAsync(new RequestEmpty() { }, MData);
                if (ResponseLogOut.Status.StatusCode == global::StatusCode.Status200 && ResponseLogOut.Status.StatusMessage == global::StatusMessage.Success)
                {
                    return Ok(new ResponseAuthorizeLogOut { CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });
                }
                return Ok(new ResponseAuthorizeLogOut { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestFailt });
            }
            catch (Exception)
            {
                return Ok(new ResponseAuthorizeLogOut { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.Error });
            }

        }

        [AllowAnonymous]
        [Route("InfoUser")]
        [HttpPost]
        public async Task<IActionResult> InfoUser([FromBody] RequestAuthorizeLogOut BodyLogOut)
        {
            try
            {
                var ClientUC = UcClient.GetChannel("http://127.0.0.1", 8080);
                var _Token = AuthenticationHttpContext.GetToken(this.HttpContext);
                Metadata MData = new Metadata();
                MData.Add("authorization", _Token);
                var ResponseUser = await ClientUC.GetUserAsync(new RequestEmpty() { }, MData);

                var _DtoUser = new DtoUser()
                {
                    User_ID = ResponseUser.User.UserID,
                    User_Fname = ResponseUser.User.UserFname,
                    User_Lname = ResponseUser.User.UserLname,
                    User_Mobile = ResponseUser.User.UserMobile,
                };
                if (ResponseUser.Status.StatusCode == global::StatusCode.Status200 && ResponseUser.Status.StatusMessage == global::StatusMessage.Success)
                {
                    return Ok(new ResponseInfoUser { User = _DtoUser, CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });
                }
                return Ok(new ResponseInfoUser { User = null, CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestFailt });
            }
            catch (Exception)
            {
                return Ok(new ResponseInfoUser { User = null, CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.Error });
            }

        }

        [AllowAnonymous]
        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RequestInsertUser BodyUser)
        {
            try
            {
                var ClientUC = UcClient.GetChannel("http://127.0.0.1", 8080);
                Google.Protobuf.WellKnownTypes.Timestamp protoTimestamp = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.Now);
                var _ResponseInsertUser = await ClientUC.InsertUserAsync(new RequestInsert()
                {
                    User = new global::User()
                    {
                        UserFname = BodyUser.User_Fname,
                        UserLname = BodyUser.User_Lname,
                        UserCodeMeli = BodyUser.User_CodeMeli,
                        UserMobile = BodyUser.User_Mobile,
                        UserAddress = BodyUser.User_Address,
                        UserDateTime = protoTimestamp,
                        UserIsActive = true,
                    }
                });

                if (_ResponseInsertUser.Status.StatusCode == global::StatusCode.Status200 && _ResponseInsertUser.Status.StatusMessage == global::StatusMessage.Success)
                {
                    return Ok(new ResponseInsertUser { CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });

                }
                return Ok(new ResponseInsertUser { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestFailt });

            }
            catch (Exception)
            {

                return Ok(new ResponseInsertUser { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.Error });

            }

        }


        

    }
}
