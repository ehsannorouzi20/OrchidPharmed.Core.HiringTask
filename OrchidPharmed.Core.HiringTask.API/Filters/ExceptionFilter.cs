using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace OrchidPharmed.Core.HiringTask.API.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception != null)
            {
                var err = context.Exception;
                Structure.APIResponse res = new Structure.APIResponse();
                res.ErrorFlag = true;
                res.Refused = false;
                res.ResultCode = 500;
                res.ResultObject = null;
                res.ResultText = err.InnerException != null ? err.InnerException.Message : err.Message;
                res.ResultTextAlias = null;
                context.Result = new ContentResult()
                {
                    Content = JsonConvert.SerializeObject(res),
                    ContentType = "application/json",
                    StatusCode = 500
                };
            }
        }
    }
}
