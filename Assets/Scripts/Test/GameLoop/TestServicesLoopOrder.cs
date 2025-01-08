using MasterProject.Tests;
using System;
using System.Collections.Generic;

namespace MasterProject
{
    /// <summary>
    /// Container class allowing us to keep track of every services and their orders
    /// </summary>
    public static class TestServicesLoopOrder
    {
        public static readonly List<Type> InitializeServicesOrder = new List<Type>()
        {
            typeof(IErrorService),
            typeof(IMusicService),
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
