using System;
using System.Collections.Generic;
using UnityEngine;

namespace MasterProject.FSM
{
    public class MonoBehaviourFSM<T> : MonoBehaviour, IFSMController where T : Enum
    {
        protected IFSMController m_Controller => this;

        public float StateDuration { get; private set; }

        protected Dictionary<T, FSMState<T>> m_States;

        protected FSMState<T> m_PreviousState;
        protected FSMState<T> m_NextState;

        protected FSMState<T> m_CurrentState;

        public virtual void Awake()
        {
            foreach (KeyValuePair<T, FSMState<T>> kvp in m_States)
            {
                kvp.Value.Initialize();
            }
        }

        public virtual void Update()
        {
            float deltaTime = Time.deltaTime;
            m_CurrentState?.Update(deltaTime);
            StateDuration += deltaTime;
        }

        public void StartFSM(T startingIndex)
        {
            if (enabled)
            {
                return;
            }
            if (m_States.TryGetValue(startingIndex, out FSMState<T> state))
            {
                m_CurrentState = state;
                StateDuration = 0f;
                enabled = true;
            }
            else
            {
                // DEBUG HERE
            }
        }

        public void StopFSM()
        {
            if (!enabled)
            {
                return;
            }
            enabled = false;
            m_PreviousState = m_CurrentState;
            m_CurrentState = null;
        }

        public bool ChangeState(T newState)
        {
            if (!enabled)
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
                // DEBUG HERE
                return null;
            }
        }
    }
}
