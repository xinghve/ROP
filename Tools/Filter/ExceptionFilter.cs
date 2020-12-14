using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools;

namespace Tools.Filter
{
    public class ExceptionFilter : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            //Logger.Default.ProcessError(500, context.Exception.Message);
            //throw new NotImplementedException();
        }
    }
}
