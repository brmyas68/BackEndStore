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
using Store.Service.ProtoBuffer.Account;
using Store.ClassDTO.DTOs.Request.Account;
using Store.ClassDTO.DTOs.Response.Account;
using Grpc.Core;

namespace Store.WebApi.Controllers.Api.Admin
{
    [EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private IUnitOfWorkStoreService _UnitOfWorkStoreService;
        public AccountController(IUnitOfWorkStoreService UnitOfWorkStoreService) { _UnitOfWorkStoreService = UnitOfWorkStoreService; }

        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("GetAll")]
        [HttpPost]
        public async Task<IActionResult> GetAll([FromBody] RequestGetAllAccount BodyAccount)
        {
            var _ResponseGetAllAccount = new ResponseGetAllAccount();
            try
            {
                var ClientAccount = AccountClient.GetChannel("http://127.0.0.1:", 8888);
                using (var ResponseAccounts = ClientAccount.GetUserAccounts(new RequestUserAccount() { UserID = BodyAccount.User_ID }))
                {
                    var _ResponseAccounts = ResponseAccounts.ResponseStream;
                    while (await _ResponseAccounts.MoveNext())
                    {
                        _ResponseGetAllAccount.Accounts.Add(
                            new DtoAccount()
                            {
                                Account_ID = _ResponseAccounts.Current.Account.AccountID,
                                Account_OrderID = _ResponseAccounts.Current.Account.AccountOrderID,
                                Account_UserID = _ResponseAccounts.Current.Account.AccountUserID,
                                Account_Price = _ResponseAccounts.Current.Account.AccountPrice,
                                Account_TypePay = _ResponseAccounts.Current.Account.AccountTypePay,
                                Account_DateTime = _ResponseAccounts.Current.Account.AccountDateTime,

                            });
                    }
                    _ResponseGetAllAccount.MessageStatus = MessageException.MessagesStatus.Success;
                    _ResponseGetAllAccount.CodeStatus = MessageException.CodeStatus.Status200;

                }


            }
            catch (Exception)
            {
                _ResponseGetAllAccount.Accounts = null;
                _ResponseGetAllAccount.MessageStatus = MessageException.MessagesStatus.RequestFailt;
                _ResponseGetAllAccount.CodeStatus = MessageException.CodeStatus.Status400;

            }
            return Ok(_ResponseGetAllAccount);
        }

        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("Insert")]
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] RequestInsertAccount BodyAccount)
        {
            var _ResponseInsertAccount = new ResponseInsertAccount();
            try
            {
                var ClientAccount = AccountClient.GetChannel("http://127.0.0.1:", 8888);
                var _ResponseAccount = await ClientAccount.InsertAccountAsync(new RequestAccount()
                {
                    Account = new IAccount()
                    {
                        AccountID = 0,
                        AccountOrderID = BodyAccount.Account_OrderID,
                        AccountUserID = BodyAccount.Account_UserID,
                        AccountDateTime = DateTime.Now.ToString(),
                        AccountPrice = BodyAccount.Account_Price,
                        AccountTypePay = BodyAccount.Account_TypePay,

                    }
                });

                _ResponseInsertAccount.Account_ID = _ResponseAccount.AccountID;
                _ResponseInsertAccount.MessageStatus = MessageException.MessagesStatus.Success;
                _ResponseInsertAccount.CodeStatus = MessageException.CodeStatus.Status200;
            }
            catch (Exception)
            {
                _ResponseInsertAccount.Account_ID = -1;
                _ResponseInsertAccount.MessageStatus = MessageException.MessagesStatus.Success;
                _ResponseInsertAccount.CodeStatus = MessageException.CodeStatus.Status200;
            }

            return Ok(_ResponseInsertAccount);

        }

    }
}
