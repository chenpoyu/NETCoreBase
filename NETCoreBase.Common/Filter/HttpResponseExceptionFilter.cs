using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using NETCoreBase.Common.Exceptions;

namespace NETCoreBase.Common.Filter
{
    public class HttpResponseExceptionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is MessageException exception)
            {
                context.Result = new JsonResult(new
                {
                    msg = exception.Message
                })
                {
                    StatusCode = exception.Status,
                };
                context.ExceptionHandled = true;
            }
            else if (context.Exception is CustomValidationException cvexception)
            {
                context.Result = new JsonResult(new
                {
                    msg = "篩選條件有誤",
                    errors = cvexception.ModelErrors
                })
                {
                    StatusCode = 400,
                };
                context.ExceptionHandled = true;
            }
            else if (context.Exception is DbUpdateConcurrencyException)
            {
                context.Result = new ObjectResult(new ProblemDetails
                {
                    Status = 409,
                    Title = "資料已被其他人修改，請重新整理後再試",
                    Type = "https://httpstatuses.com/409"
                })
                {
                    StatusCode = 409,
                };
                context.ExceptionHandled = true;
            }
            else if (context.Exception != null)
            {
                context.Result = new JsonResult(new
                {
                    //msg = "系統忙碌中請稍後再試"
                    msg = context.Exception.Message
                })
                {
                    StatusCode = 500,
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
