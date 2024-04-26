using MasterProject.FSM;
using UnityEngine;

public class TestFSM : MonoBehaviour, IFSMController
{
    [SerializeField] private Movable2D m_Movable;

    private MovementFSM m_FSM;

    void Start()
    {
        m_FSM = new MovementFSM(this, m_Movable);
        m_FSM.Initialize();
        m_FSM.Start(MovementStateID.Idle);
    }

    void Update()
    {
        m_FSM?.Update(Time.deltaTime);
    }
}
