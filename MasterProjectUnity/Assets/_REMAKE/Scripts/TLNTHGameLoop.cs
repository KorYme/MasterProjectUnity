using MasterProject;
using MasterProject.Services;
using MasterProject.Tests;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TLNTH
{
    public class TLNTHGameLoop : BaseGameLoop
    {
        [SerializeField] private InputService m_inputService;
        [SerializeField] private MusicService m_musicService;
        [SerializeField] private ErrorService m_errorService;
        [SerializeField] private SceneLoaderService m_sceneLoaderService;

        protected override IReadOnlyList<Type> InitializeServicesOrder => TLNTHServicesLoopOrder.InitializeServicesOrder;

        protected override IReadOnlyList<Type> UpdateServicesOrder => TLNTHServicesLoopOrder.UpdateServicesOrder;

        protected override IReadOnlyList<Type> LateUpdateServicesOrder => TLNTHServicesLoopOrder.LateUpdateServicesOrder;

        protected override void InstantiateServices(in BindingContainer container)
        {
            container.BindWithPrefab<IInputService>(m_inputService);
            container.BindWithPrefab<IMusicService>(m_musicService);
            container.BindWithPrefab<IErrorService>(m_errorService);
            container.BindWithInstance<ISceneLoaderService>(m_sceneLoaderService);
        }
    }
}
