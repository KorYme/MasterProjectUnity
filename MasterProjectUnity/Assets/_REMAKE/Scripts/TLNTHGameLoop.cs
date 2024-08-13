using MasterProject;
using MasterProject.Tests;
using MasterProject.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TLNTH
{
    public class TLNTHGameLoop : BaseGameLoop
    {
        [SerializeField] private InputService m_inputService;

        [SerializeField] private TLNTHCharacterController m_characterController;

        protected override IReadOnlyList<Type> InitializeServicesOrder => TLNTHServicesLoopOrder.InitializeServicesOrder;

        protected override IReadOnlyList<Type> UpdateServicesOrder => TLNTHServicesLoopOrder.UpdateServicesOrder;

        protected override IReadOnlyList<Type> LateUpdateServicesOrder => TLNTHServicesLoopOrder.LateUpdateServicesOrder;

        protected override void GenerateScenes()
        {
        }

        protected override void InstantiateServices(in BindingContainer container)
        {
            container.BindWithInstance<IInputService>(m_inputService);
            container.BindWithMockService<IMusicService>();
            container.BindWithMockService<IErrorService>();
        }

        protected override void SetupOtherDependencies()
        {
            InjectionUtilities.InjectDependencies(typeof(ServiceDepencency), m_container.AllServices, m_characterController);
        }
    }
}
