using Unity.VisualScripting;
using UnityEngine;

namespace Phone
{
    public class SleepDemo : MonoBehaviour
    {
        void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}