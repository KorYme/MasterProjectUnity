using MasterProject.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MasterProject.Tests
{
    public class MockGameLoop : BaseGameLoop
    {
        [SerializeField] private TestComponentWithInjection m_test;

        protected override IReadOnlyList<Type> InitializeServicesOrder => TestServicesLoopOrder.InitializeServicesOrder;

        protected override IReadOnlyList<Type> UpdateServicesOrder => TestServicesLoopOrder.UpdateServicesOrder;

        protected override IReadOnlyList<Type> LateUpdateServicesOrder => TestServicesLoopOrder.LateUpdateServicesOrder;

        protected override void InstantiateServices(in BindingContainer container)
        {
            container.BindWithMockService<IErrorService>();
            container.BindWithMockService<IMusicService>();
        }

        protected override void SetupOtherDependencies()
        {
            InjectionUtilities.InjectDependencies(m_test, typeof(ServiceDepencency), m_container.AllServices);
        }
    }
}
