using System.Collections;
using Prime31;
using UnityEngine;

namespace Game.Misc
{
    public class Movement : MonoBehaviour
    {
        protected EntityMovement Controller;
        
        public delegate void OnToFall(float hp);

        public event OnToFall OnToFallEvent;
        
        protected IEnumerator CheckFallHp()
        {
            var startPos = Vector3.zero;
            var started = false;
		
            while (true)
            {
                yield return null;
                var isGround = Controller.isGrounded;
                if (!isGround && !started)
                {
                    started = true;
                    startPos = transform.position;
                    //Debug.Log("startpos: " + startPos);
                }
                
                if (isGround && started)
                {
                    started = false;
                    var pos = transform.position;
                    if (pos.y < startPos.y)
                    {
                        var distance = Vector3.Distance(startPos, pos);
                        const float minDistance = 3f;
                        if (distance >= minDistance)
                        {
                            //Debug.Log("startpos: " + startPos + " ;distance" + distance + " ;pos" + pos);
                            distance = distance - minDistance;
                            var hp = -(Mathf.Floor(distance) * Random.Range(4, 6));
                        
                            OnToFallEvent?.Invoke(hp);
                        }
                    }
                }
            }
        }
    }
}