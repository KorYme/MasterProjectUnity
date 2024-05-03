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
        public Movable2D Movable;

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

    [Serializable]
    public class IdleState : FSMState<MovementStateID>
    {
        [SerializeField] private AnimationClip m_AnimationClip;

        public IdleState(FSM<MovementStateID> fsm) : base(fsm)
        {
        }

        public override void Enter()
        {
            base.Enter();

        }
    }

    [Serializable]
    public class WalkState : FSMState<MovementStateID>
    {
        public WalkState(FSM<MovementStateID> fsm) : base(fsm)
        {
        }
    }

    [Serializable]
    public class AccelerateState : FSMState<MovementStateID>
    {
        public AccelerateState(FSM<MovementStateID> fsm) : base(fsm)
        {
        }
    }

    [Serializable]
    public class RunState : FSMState<MovementStateID>
    {
        public RunState(FSM<MovementStateID> fsm) : base(fsm)
        {
        }
    }

    [Serializable]
    public class DecelerateState : FSMState<MovementStateID>
    {
        public DecelerateState(FSM<MovementStateID> fsm) : base(fsm)
        {
        }
    }
}

