using UnityEngine;

namespace Game
{
    public class TimerToDelete : MonoBehaviour
    {
        public enum Mode
        {
            Destroy,
            Disable
        }
        [SerializeField] private float time;
        [SerializeField] private Mode mode;
        
        private IEnumerator StartDeleteting()
        {
        
        }
    }
}