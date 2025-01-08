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
        private const int DEFAULT_ORDER = 1000;

        [SerializeField] CanvasOrderID m_canvasID;

        private Canvas m_canvas;

        private void Awake()
        {
            m_canvas = GetComponent<Canvas>();
            m_canvas.sortingOrder = (int)m_canvasID + DEFAULT_ORDER;
        }
    }
}
