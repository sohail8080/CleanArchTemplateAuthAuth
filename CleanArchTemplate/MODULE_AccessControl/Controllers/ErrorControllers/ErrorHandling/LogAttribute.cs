using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CleanArchTemplate.MODULE_AccessControl.Controllers.ErrorControllers.ErrorHandling
{
    //[Serializable]
    //class LogAttribute : OnMethodBoundaryAspect
    //{
    //    static readonly ILog log = LogManager.GetLogger(typeof(MainClass));
    //    public override void OnEntry(MethodExecutionArgs args)
    //    {
    //        log.InfoFormat("Entering {0}.{1}.", args.Method.DeclaringType.Name, args.Method.Name);
    //        log.Debug(DisplayObjectInfo(args));
    //    }

    //    public override void OnExit(MethodExecutionArgs args)
    //    {
    //        log.DebugFormat("Leaving {0}.{1}. Return value {2}", args.Method.DeclaringType.Name, args.Method.Name, args.ReturnValue);
    //    }

    //    public override void OnException(MethodExecutionArgs args)
    //    {
    //        log.ErrorFormat("Exception {0} in {1}", args.Exception, args.Method);
    //        args.FlowBehavior = FlowBehavior.Continue;
    //    }

    //    static string DisplayObjectInfo(MethodExecutionArgs args)
    //    {
    //        StringBuilder sb = new StringBuilder();
    //        Type type = args.Arguments.GetType();
    //        sb.Append("Method " + args.Method.Name);
    //        sb.Append("\r\nArguments:");
    //        FieldInfo[] fi = type.GetFields();
    //        if (fi.Length > 0)
    //        {
    //            foreach (FieldInfo f in fi)
    //            {
    //                sb.Append("\r\n " + f + " = " + f.GetValue(args.Arguments));
    //            }
    //        }
    //        else
    //            sb.Append("\r\n None");

    //        return sb.ToString();
    //    }
    //}
}