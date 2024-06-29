using MasterProject.FSM;
using UnityEngine;

namespace MasterProject.Tests.FSM
{
    public class TestFSM : MonoBehaviour, IFSMController
    {
        [SerializeField] private MovementFSM m_fsm;

        void Start()
        {
            m_fsm = new MovementFSM(this);
            m_fsm.Initialize();
            m_fsm.Start(MovementStateID.Idle);
        }

        void Update()
        {
            m_fsm?.Update(Time.deltaTime);
        }
    }
}