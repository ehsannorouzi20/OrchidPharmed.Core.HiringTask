using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace OrchidPharmed.Core.HiringTask.API.Filters
{
    public class UniformResultFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var result = context.Result as ObjectResult;
            var customResponse = new Structure.APIResponse
            {
                ErrorFlag = false,
                ExtraCommands = null,
                Refused = false,
                ResultCode =  200 ,
                ResultText = null,
                ResultTextAlias = null,
                TokenExpired = false
            };
            if (result != null) 
                customResponse.ResultObject = result.Value;
            context.Result = new ObjectResult(customResponse);
            base.OnActionExecuted(context);
        }
    }
}
