using Photon.Pun;
using UnityEngine;

public class CarController : MonoBehaviour
{
    void Update()
    {
        if (PhotonNetwork.IsMasterClient) // Der Host steuert die Beschleunigung
        {
            float acceleration = Input.GetAxis("Vertical");
            // Logik für die Beschleunigung
        }
        else // Der andere Spieler steuert die Richtung
        {
            float steering = Input.GetAxis("Horizontal");
            // Logik für die Lenkung
        }
    }
}
