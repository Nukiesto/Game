using Photon.Pun;
using UnityEngine;

namespace Game.Misc
{
    public class MultiPlayerManager : MonoBehaviourPunCallbacks
    {
        public bool IsConnectedToMaster;


        public override void OnConnectedToMaster()
        {
            Debug.Log("IsConnectedToMaster");
            IsConnectedToMaster = true;
        }
    }
}