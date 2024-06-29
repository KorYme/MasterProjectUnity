using MasterProject.Debugging;
using System;
using System.Collections.Generic;

namespace MasterProject.FSM
{
    public class FSM<T> where T : Enum
    {
        [NonSerialized]
        private IFSMController m_controller;

        private Dictionary<T, FSMState<T>> m_states = new Dictionary<T, FSMState<T>>();

        [field: NonSerialized]
        public FSMState<T> PreviousState { get; private set; }

        [field: NonSerialized]
        public FSMState<T> CurrentState { get; private set; }

        public float StateDuration { get; private set; }

        public bool IsFSMRunning {  get; private set; }

        public FSM(IFSMController controller)
        {
            m_controller = controller;
        }

        public virtual void Initialize()
        {
            foreach (KeyValuePair<T, FSMState<T>> kvp in m_states)
            {
                kvp.Value.Initialize();
            }
        }

        public virtual void Update(float deltaTime)
        {
            if (!IsFSMRunning)
            {
                return;
            }
            CurrentState?.Update(deltaTime);
            StateDuration += deltaTime;
        }

        public void Start(T startingIndex)
        {
            if (IsFSMRunning)
            {
                return;
            }
            if (m_states.TryGetValue(startingIndex, out FSMState<T> state))
            {
                CurrentState = state;
                StateDuration = 0f;
                IsFSMRunning = true;
            }
            else
            {
                DebugLogger.Error(GetType().Name, $"No state {startingIndex} has been found and started.");
            }
        }

        public void Stop()
        {
            if (!IsFSMRunning)
            {
                return;
            }
            IsFSMRunning = false;
            PreviousState = CurrentState;
            CurrentState = null;
        }

        public bool ChangeState(T newState)
        {
            if (!IsFSMRunning)
            {
                return false;
            }
            if (m_states.TryGetValue(newState, out FSMState<T> state) && CurrentState != state)
            {
                CurrentState.Exit();
                PreviousState = CurrentState;
                CurrentState = state;
                CurrentState.Enter();
                StateDuration = 0f;
                return true;
            }
            return false;
        }

        public bool AddState(T index, FSMState<T> state)
        {
            return m_states.TryAdd(index, state);
        }

        public TState AddAndGetState<TState>(T index) where TState : FSMState<T>
        {
            if (!m_states.ContainsKey(index))
            {
                TState state = (TState)Activator.CreateInstance(typeof(TState), this);
                m_states.Add(index, state);
                return state;
            }
            else
            {
                DebugLogger.Warning(this, $"A state {index} has already been added to the FSM.");
                return null;
            }
        }

        public bool TryGetController<TFSMController>(out TFSMController controller) where TFSMController : IFSMController
        {
            if (m_controller is TFSMController fsmController)
            {
                controller = fsmController;
                return true;
            }
            controller = default;
            return false;
        }
    }
}
