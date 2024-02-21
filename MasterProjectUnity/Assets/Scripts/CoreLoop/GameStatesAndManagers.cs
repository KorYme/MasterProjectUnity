using MasterProject.Managers;
using System;
using System.Collections.Generic;

namespace MasterProject
{
    /// <summary>
    /// Container class allowing us to keep track of every managers and their order in gameloop differents functions
    /// </summary>
    public class GameStatesAndManagers
    {
        public static readonly List<Type> AllManagers = new List<Type>()
        {
            
        };

        public static readonly List<Type> InitializeManagersOrder = new List<Type>()
        {

        };

        public static readonly List<Type> UpdateManagersOrder = new List<Type>()
        {

        };
    }
}
