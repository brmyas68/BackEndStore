


using Store.InterfaceService.InterfacesBase;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

using Store.Common.Exceptions;

using Store.ClassDTO.DTOs.Request.Log;
using Store.ClassDTO.DTOs.Response.Log;
using Store.Service.ProtoBuffer.Log;
using ProtoLogService;
using Grpc.Core;


namespace Store.WebApi.Controllers.Api.Admin
{
    [EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private IUnitOfWorkStoreService _UnitOfWorkStoreService;
        public LogsController(IUnitOfWorkStoreService UnitOfWorkStoreService) { _UnitOfWorkStoreService = UnitOfWorkStoreService; }

        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("GetAll")]
        [HttpPost]
        public async Task<IActionResult> GetAll([FromBody] RequestGetAllLog BodyLog)
        {

            var _ResponseGetAllLog = new ResponseGetAllLog();

            try
            {
                var ClientLog = LogClient.GetChannel("http://127.0.0.1:", 8000);
                using (var ResponseLogs = ClientLog.GetByWhere(new RequestWhereLog() { LogUserID = BodyLog.Log_UserID }))
                {
                    var _ResponseLog = ResponseLogs.ResponseStream;
                    while (await _ResponseLog.MoveNext())
                    {
                        _ResponseGetAllLog.Logs.Add(
                            new ClassDTO.DTOs.DtoLog()
                            {
                                Log_ID = _ResponseLog.Current.Log.LogID,
                                Log_Action = _ResponseLog.Current.Log.LogAction,
                                Log_Controller = _ResponseLog.Current.Log.LogController,
                                Log_DateTime = _ResponseLog.Current.Log.LogDateTime,
                                Log_Message = _ResponseLog.Current.Log.LogMessage,
                                Log_UserID = _ResponseLog.Current.Log.LogUserID,
                            });
                    }
                    _ResponseGetAllLog.MessageStatus = MessageException.MessagesStatus.Success;
                    _ResponseGetAllLog.CodeStatus = MessageException.CodeStatus.Status200;

                }
            }
            catch (Exception)
            {
                _ResponseGetAllLog.Logs = null;
                _ResponseGetAllLog.MessageStatus = MessageException.MessagesStatus.RequestFailt;
                _ResponseGetAllLog.CodeStatus = MessageException.CodeStatus.Status400;
            }


            return Ok(_ResponseGetAllLog);
        }

        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("Insert")]
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] RequestInsertLogs BodyLog)
        {

            try
            {
                var ClientLog = LogClient.GetChannel("http://127.0.0.1:", 8000);
                var _ResponseInsertLogs = await ClientLog.InsertAsync(new RequestInsertLog()
                {
                    Log = new Log()
                    {
                        LogAction = BodyLog.LogAction,
                        LogController = BodyLog.LogController.ToString(),
                        LogDateTime = DateTime.Now.ToString(),
                        LogMessage = BodyLog.LogMessage,
                        LogUserID = BodyLog.LogUserID,
                    }

                });

                if (_ResponseInsertLogs.LogState)
                {
                    return Ok(new ResponseInsertLogs { CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });

                }
                return Ok(new ResponseInsertLogs { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestFailt });


            }
            catch (Exception)
            {
                return Ok(new ResponseInsertLogs { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.Error });

            }

        }
    }
}
