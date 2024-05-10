using MasterProject.Debugging;
using System;
using System.Collections.Generic;

namespace MasterProject.FSM
{
    public class FSM<T> where T : Enum
    {
        private IFSMController m_Controller;

        private Dictionary<T, FSMState<T>> m_States = new Dictionary<T, FSMState<T>>();

        public FSMState<T> PreviousState { get; private set; }

        public FSMState<T> CurrentState { get; private set; }

        public float StateDuration { get; private set; }

        public bool IsFSMRunning {  get; private set; }

        public FSM(IFSMController controller)
        {
            m_Controller = controller;
        }

        public virtual void Initialize()
        {
            foreach (KeyValuePair<T, FSMState<T>> kvp in m_States)
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
            if (m_States.TryGetValue(startingIndex, out FSMState<T> state))
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
            if (m_States.TryGetValue(newState, out FSMState<T> state) && CurrentState != state)
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
            return m_States.TryAdd(index, state);
        }

        public TState AddAndGetState<TState>(T index) where TState : FSMState<T>
        {
            if (!m_States.ContainsKey(index))
            {
                TState state = (TState)Activator.CreateInstance(typeof(TState), this);
                m_States.Add(index, state);
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
            if (m_Controller is TFSMController fsmController)
            {
                controller = fsmController;
                return true;
            }
            controller = default;
            return false;
        }
    }
}
