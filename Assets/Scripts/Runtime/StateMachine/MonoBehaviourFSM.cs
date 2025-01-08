using System;
using System.Collections.Generic;
using UnityEngine;

namespace MasterProject.FSM
{
    public class MonoBehaviourFSM<T> : MonoBehaviour, IFSMController where T : Enum
    {
        protected IFSMController Controller => this;

        public float StateDuration { get; private set; }

        protected Dictionary<T, FSMState<T>> m_states;

        protected FSMState<T> m_previousState;
        protected FSMState<T> m_nextState;

        protected FSMState<T> m_currentState;

        public virtual void Awake()
        {
            foreach (KeyValuePair<T, FSMState<T>> kvp in m_states)
            {
                kvp.Value.Initialize();
            }
        }

        public virtual void Update()
        {
            float deltaTime = Time.deltaTime;
            m_currentState?.Update(deltaTime);
            StateDuration += deltaTime;
        }

        public void StartFSM(T startingIndex)
        {
            if (enabled)
            {
                return;
            }
            if (m_states.TryGetValue(startingIndex, out FSMState<T> state))
            {
                m_currentState = state;
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
            m_previousState = m_currentState;
            m_currentState = null;
        }

        public bool ChangeState(T newState)
        {
            if (!enabled)
            {
                return false;
            }
            if (m_states.TryGetValue(newState, out FSMState<T> state) && m_currentState != state)
            {
                m_currentState.Exit();
                m_previousState = m_currentState;
                m_currentState = state;
                m_currentState.Enter();
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
                // DEBUG HERE
                return null;
            }
        }
    }
}
