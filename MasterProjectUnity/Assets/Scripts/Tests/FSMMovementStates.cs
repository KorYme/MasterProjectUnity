namespace MasterProject.FSM
{
    public enum MovementStateID
    {
        Idle,
        Walk,
        Accelerate,
        Run,
        Decelerate,
    }

    public class MovementFSM : FSM<MovementStateID>
    {
        public IdleState IdleState { get; protected set; }
        public WalkState WalkState { get; protected set; }
        public AccelerateState AccelerateState { get; protected set; }
        public RunState RunState { get; protected set; }
        public DecelerateState DecelerateState { get; protected set; }

        public Movable2D Movable2D { get; protected set; }

        public MovementFSM(IFSMController controller, Movable2D movable2D) : base(controller)
        {
            Movable2D = movable2D;
            IdleState = AddAndGetState<IdleState>(MovementStateID.Idle);
            WalkState = AddAndGetState<WalkState>(MovementStateID.Walk);
            AccelerateState = AddAndGetState<AccelerateState>(MovementStateID.Accelerate);
            RunState = AddAndGetState<RunState>(MovementStateID.Run);
            DecelerateState = AddAndGetState<DecelerateState>(MovementStateID.Run);
        }
    }

    public class IdleState : FSMState<MovementStateID>
    {
        public IdleState(FSM<MovementStateID> fsm) : base(fsm)
        {
        }

        public override void Enter()
        {
            base.Enter();

        }
    }

    public class WalkState : FSMState<MovementStateID>
    {
        public WalkState(FSM<MovementStateID> fsm) : base(fsm)
        {
        }
    }

    public class AccelerateState : FSMState<MovementStateID>
    {
        public AccelerateState(FSM<MovementStateID> fsm) : base(fsm)
        {
        }
    }

    public class RunState : FSMState<MovementStateID>
    {
        public RunState(FSM<MovementStateID> fsm) : base(fsm)
        {
        }
    }

    public class DecelerateState : FSMState<MovementStateID>
    {
        public DecelerateState(FSM<MovementStateID> fsm) : base(fsm)
        {
        }
    }
}

