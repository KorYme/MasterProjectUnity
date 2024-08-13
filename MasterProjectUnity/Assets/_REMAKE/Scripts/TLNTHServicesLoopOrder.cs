using MasterProject.Tests;
using System;
using System.Collections.Generic;

namespace TLNTH
{
    public static class TLNTHServicesLoopOrder
    {
        public static readonly List<Type> InitializeServicesOrder = new List<Type>()
        {
            typeof(IErrorService),
            typeof(IMusicService),
            typeof(IInputService),
        };

        public static readonly List<Type> UpdateServicesOrder = new List<Type>()
        {
            typeof(IMusicService),
        };

        public static readonly List<Type> LateUpdateServicesOrder = new List<Type>()
        {
            typeof(IErrorService),
        };
    }
}
