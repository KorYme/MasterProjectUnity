using System;
using System.Collections.Generic;

namespace MasterProject.Tests
{
    public class MockGameLoop : BaseGameLoop
    {
        protected override IReadOnlyList<Type> InitializeServicesOrder => TestServicesLoopOrder.InitializeServicesOrder;

        protected override IReadOnlyList<Type> UpdateServicesOrder => TestServicesLoopOrder.UpdateServicesOrder;

        protected override IReadOnlyList<Type> LateUpdateServicesOrder => TestServicesLoopOrder.LateUpdateServicesOrder;

        protected override void InstantiateServices(in BindingContainer container)
        {
            container.BindWithMockService<IErrorService>();
            container.BindWithMockService<IMusicService>();
        }
    }
}
