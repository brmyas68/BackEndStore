
using AutoMapper;

using Store.InterfaceService.InterfacesBase;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Store.Service.ProtoBuffer.Comment;
using ProtoCommentService;
using Grpc.Core;
using Store.ClassDTO.DTOs.Response;
using Store.Common.Exceptions;
using Store.ClassDTO.DTOs.Request.Comment;
using Store.ClassDTO.DTOs.Response.Comment;

namespace Store.WebApi.Controllers.Api.Admin
{
    [EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private IUnitOfWorkStoreService _UnitOfWorkStoreService;
        public CommentController(IUnitOfWorkStoreService UnitOfWorkStoreService, IConfiguration Configuration) { _UnitOfWorkStoreService = UnitOfWorkStoreService; }

        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("GetAll")]
        [HttpPost]
        public async Task<IActionResult> GetAll([FromBody] RequestGetAllComment BodyComment)
        {
            var _ResponseGetAllComment = new ResponseGetAllComment();

            try
            {
                var ClientComment = CommentClient.GetChannel("http://127.0.0.1:", 5000);
                using (var ResponseComments = ClientComment.GetByWhere(new RequestWhereComment() { CommentProductID = BodyComment.Comment_ProductID, CommentUserID = BodyComment.Comment_UserID }))
                {
                    var _ResponseComment = ResponseComments.ResponseStream;
                    while (await _ResponseComment.MoveNext())
                    {
                        _ResponseGetAllComment.Comments.Add(
                            new ClassDTO.DTOs.DtoComment()
                            {
                                Comment_ID = _ResponseComment.Current.Comment.CommentID,
                                Comment_ProductID = Convert.ToInt32(_ResponseComment.Current.Comment.CommentProductID),
                                Comment_UserID = _ResponseComment.Current.Comment.CommentUserID,
                                Comment_Text = _ResponseComment.Current.Comment.CommentText,
                                Comment_DateTime = _ResponseComment.Current.Comment.CommentDateTime,
                            });
                    }
                    _ResponseGetAllComment.MessageStatus = MessageException.MessagesStatus.Success;
                    _ResponseGetAllComment.CodeStatus = MessageException.CodeStatus.Status200;

                }
            }
            catch (Exception)
            {
                _ResponseGetAllComment.Comments = null;
                _ResponseGetAllComment.MessageStatus = MessageException.MessagesStatus.RequestFailt;
                _ResponseGetAllComment.CodeStatus = MessageException.CodeStatus.Status400;
            }


            return Ok(_ResponseGetAllComment);

        }

        // [AuthorizePermission(EnumPermission.Controllers.Form_UC_User, EnumPermission.Actions.Action_UC_User_Update)]
        [Route("Insert")]
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] RequestInsertComments BodyComment)
        {

            try
            {
                var ClientComment = CommentClient.GetChannel("http://127.0.0.1:", 5000);
                var _ResponseInsertComments = await ClientComment.InsertAsync(new RequestInsertComment()
                {
                    Comment = new Comment()
                    {
                        CommentDateTime = DateTime.Now.ToString(),
                        CommentProductID = BodyComment.CommentProductID.ToString(),
                        CommentText = BodyComment.CommentText,
                        CommentUserID = BodyComment.CommentUserID,
                    }

                });

                if (_ResponseInsertComments.CommentState)
                {
                    return Ok(new ResponseInsertComments { CodeStatus = MessageException.CodeStatus.Status200, MessageStatus = MessageException.MessagesStatus.Success });

                }
                return Ok(new ResponseInsertComments { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.RequestFailt });


            }
            catch (Exception)
            {
                return Ok(new ResponseInsertComments { CodeStatus = MessageException.CodeStatus.Status400, MessageStatus = MessageException.MessagesStatus.Error });

            }

        }
    }
}
