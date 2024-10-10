using AutoMapper;
using ICMD.API.Helpers;
using ICMD.Core.Account;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.TagField;
using ICMD.Core.Shared.Extension;
using ICMD.Core.Shared.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ICMD.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class TagFieldController : BaseController
    {
        private readonly ITagFieldService _tagFieldService;
        private readonly IMapper _mapper;
        private readonly CommonMethods _commonMethods;
        private static string ModuleName = "Tag field";
        public TagFieldController(IMapper mapper, ITagFieldService tagFieldService, CommonMethods commonMethods)
        {
            _tagFieldService = tagFieldService;
            _mapper = mapper;
            _commonMethods = commonMethods;
        }

        #region TagField
        [HttpGet]
        [AuthorizePermission()]
        public async Task<List<TagFieldInfoDto>> GetProjectTagFieldsInfo(Guid projectId)
        {
            bool isEditable = _commonMethods.TaxFieldEditableOrNot(User.GetUserId(), projectId);
            List<TagField>? tagFields = await _tagFieldService.GetAll(s => s.IsActive && !s.IsDeleted && s.ProjectId == projectId).OrderBy(s => s.Position).ToListAsync();
            List<TagFieldInfoDto> data = _mapper.Map<List<TagFieldInfoDto>>(tagFields);
            data.ForEach(s => s.IsEditable = isEditable);
            return data;
        }

        [HttpPost]
        [AuthorizePermission(Operations.Add, Operations.Edit)]
        public async Task<BaseResponse> UpdateProjectTagFields(UpdateProjectTagFieldsDto info)
        {
            if (ModelState.IsValid)
            {
                if (info.TagFields != null && info.TagFields.Any())
                {
                    foreach (var item in info.TagFields)
                    {
                        TagField chkExist = await _tagFieldService.GetSingleAsync(s => s.Id == item.Id && s.IsActive && !s.IsDeleted);
                        if (chkExist != null)
                        {
                            TagField oldInfo = chkExist;
                            chkExist.Name = item.Name;
                            chkExist.Separator = item.Separator;
                            chkExist.Source = item.Source;
                            _tagFieldService.Update(chkExist, oldInfo, User.GetUserId());
                        }
                    }

                }
                return new BaseResponse(true, ResponseMessages.ProjectTagFieldUpdated, HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }
        #endregion
    }
}
