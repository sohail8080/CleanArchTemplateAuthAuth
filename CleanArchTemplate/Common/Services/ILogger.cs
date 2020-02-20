using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CleanArchTemplate.Common.Services
{
    public interface ILogger
    {
        void LogError(Exception ex);

    }
}