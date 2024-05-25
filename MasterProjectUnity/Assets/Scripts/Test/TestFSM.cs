using MasterProject.FSM;
using UnityEngine;
using TomatoNamespace;
using System;

namespace MasterProject.Tests.FSM
{
    public class TestFSM : MonoBehaviour, IFSMController
    {
        [SerializeField] private MovementFSM m_FSM;

        void Start()
        {
            m_FSM = new MovementFSM(this);
            m_FSM.Initialize();
            m_FSM.Start(MovementStateID.Idle);
        }

        void Update()
        {
            m_FSM?.Update(Time.deltaTime);
        }
    }
}


namespace TomatoNamespace // PascalCase for namespaces
{
    public class Tomate // PascalCase for classes
        : IFSMController // PascalCase + I at the beginning for interfaces
    {
        private const string TOMATO_PULP = ""; // UPPER_CASE for constants

        public float Prout; // Pascal case for public members

        public static Action OnGrosProut; // Pascal case for public static variables

        private float m_proutPurified; // m_ + camelCase for protected/private members

        protected static float s_prout; // s_ + camelCase for protected/private static variables

        private event Action m_onTomated; // Actions always starts by "on"

        private enum ProutTypes // PascalCase for enums
        {
            Ninja,
            Sadique,
            BonVivant,
            Rigolo
        }

        protected float GrosGrosProut { get; private set; } // For properties always use PascalCase

        public float GrosGrosGrosProut // Both conventions can be used in properties/functions/methods
        {
            get => m_proutPurified;
            set { m_proutPurified = value; }
        }

        private void Excrement() // Always write protection keyword + PascalCase for methods/functions
        {
            void Method() // For callback purpose, use local methods/functions ONLY
            {
                OnGrosProut -= Method;
            }
            OnGrosProut += Method;
        }

        private void Increment(int addition) => Prout += addition; // camelCase for parameters
    }
}