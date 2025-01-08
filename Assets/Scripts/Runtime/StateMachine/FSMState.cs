using System;

namespace MasterProject.FSM
{
    public class FSMState<T> where T : Enum
    {
        [NonSerialized]
        protected FSM<T> m_fsm;

        public FSMState(FSM<T> fsm)
        {
            m_fsm = fsm;
        }

        public virtual void Initialize()
        {
        }

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }

        public virtual void Update(float deltaTime)
        {
        }
    }
}