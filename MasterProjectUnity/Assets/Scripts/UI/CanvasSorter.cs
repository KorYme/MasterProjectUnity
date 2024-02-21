using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace MasterProject.UI
{
    public enum CanvasOrderID
    {
        Front = 1,
        Game = 0,
        Back = -1,
    }

    [RequireComponent(typeof(Canvas))]
    public class CanvasSorter : MonoBehaviour
    {
        public const int DEFAULT_ORDER = 1000;

        [SerializeField] CanvasOrderID m_CanvasID;

        Canvas canvas;

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            canvas.sortingOrder = (int)m_CanvasID + DEFAULT_ORDER;
        }
    }
}
