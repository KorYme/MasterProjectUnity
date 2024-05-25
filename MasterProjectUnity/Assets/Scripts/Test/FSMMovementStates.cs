using MasterProject.FSM;
using System;
using UnityEngine;

namespace MasterProject.Tests.FSM
{
    public enum MovementStateID
    {
        Idle,
        Walk,
        Accelerate,
        Run,
        Decelerate,
    }

    [Serializable]
    public class MovementFSM : FSM<MovementStateID>
    {
        [SerializeField] private CharacterController m_CharacterController;
        public CharacterController CharacterController => m_CharacterController;

        [SerializeField] private Animator m_Animator;
        public Animator Animator => m_Animator;

        [field:SerializeField] public IdleState IdleState { get; protected set; }
        [field:SerializeField] public WalkState WalkState { get; protected set; }
        [field:SerializeField] public AccelerateState AccelerateState { get; protected set; }
        [field:SerializeField] public RunState RunState { get; protected set; }
        [field:SerializeField] public DecelerateState DecelerateState { get; protected set; }

        public MovementFSM(IFSMController controller) : base(controller)
        {
        }

        public override void Initialize()
        {
            AddState(MovementStateID.Idle, IdleState);
            AddState(MovementStateID.Walk, WalkState);
            AddState(MovementStateID.Accelerate, AccelerateState);
            AddState(MovementStateID.Run, RunState);
            AddState(MovementStateID.Decelerate, DecelerateState);
            base.Initialize();
        }
    }

    public abstract class MovementState : FSMState<MovementStateID>
    {
        [SerializeField] private AnimationClip m_AnimationClip;

        protected MovementFSM m_MovementFSM;

        public MovementState(FSM<MovementStateID> fsm) : base(fsm)
        {
            m_MovementFSM = m_FSM as MovementFSM;
        }
    }

    [Serializable]
    public class IdleState : MovementState
    {
        public IdleState(FSM<MovementStateID> fsm) : base(fsm)
        {
        }

        public override void Enter()
        {
            base.Enter();

        }
    }

    [Serializable]
    public class WalkState : MovementState
    {
        public WalkState(FSM<MovementStateID> fsm) : base(fsm)
        {
        }
    }

    [Serializable]
    public class AccelerateState : MovementState
    {
        public AccelerateState(FSM<MovementStateID> fsm) : base(fsm)
        {
        }
    }

    [Serializable]
    public class RunState : MovementState
    {
        public RunState(FSM<MovementStateID> fsm) : base(fsm)
        {
        }
    }

    [Serializable]
    public class DecelerateState : MovementState
    {
        public DecelerateState(FSM<MovementStateID> fsm) : base(fsm)
        {
        }
    }
}

