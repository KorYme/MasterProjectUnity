
using MasterProject.Debugging;
using System;
using System.Collections.Generic;

namespace MasterProject.FSM
{
    public class FSM<T> where T : Enum
    {
        protected IFSMController m_Controller;

        protected bool m_IsFSMRunning;
        public float StateDuration { get; private set; }

        protected Dictionary<T, FSMState<T>> m_States;

        protected FSMState<T> m_PreviousState;
        protected FSMState<T> m_NextState;

        protected FSMState<T> m_CurrentState;

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
            if (!m_IsFSMRunning)
            {
                return;
            }
            m_CurrentState?.Update(deltaTime);
            StateDuration += deltaTime;
        }

        public void Start(T startingIndex)
        {
            if (m_IsFSMRunning)
            {
                return;
            }
            if (m_States.TryGetValue(startingIndex, out FSMState<T> state))
            {
                m_CurrentState = state;
                StateDuration = 0f;
                m_IsFSMRunning = true;
            }
            else
            {
                DebugLogger.Error(GetType().Name, $"No state {startingIndex} has been found and started.");
            }
        }

        public void Stop()
        {
            if (!m_IsFSMRunning)
            {
                return;
            }
            m_IsFSMRunning = false;
            m_PreviousState = m_CurrentState;
            m_CurrentState = null;
        }

        public bool ChangeState(T newState)
        {
            if (!m_IsFSMRunning)
            {
                return false;
            }
            if (m_States.TryGetValue(newState, out FSMState<T> state) && m_CurrentState != state)
            {
                m_CurrentState.Exit();
                m_PreviousState = m_CurrentState;
                m_CurrentState = state;
                m_CurrentState.Enter();
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
                DebugLogger.Warning(GetType().Name, $"A state {index} has already been added to the FSM.");
                return null;
            }
        }
    }
}
