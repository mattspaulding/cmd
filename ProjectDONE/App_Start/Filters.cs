using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace ProjectDONE.App_Start.Filters
{
    public class UnhandledExceptionFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(HttpActionExecutedContext context)
        {
            //Add detailed ELMAH logging here to Azure table storage.
            //For now, Azure diagnostics logging will work.
            //if (System.Diagnostics.Debugger.IsAttached)
            //{
            //    System.Diagnostics.Debugger.Break();
            //}
        }

    }


  
    
}