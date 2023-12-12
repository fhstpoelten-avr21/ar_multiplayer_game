using DavidJalbert;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerSetup : MonoBehaviourPun
    {

        public TextMeshProUGUI playerNameText;
        public GameObject carController;
        public GameObject mobileUI;

        // Start is called before the first frame update
        void Start()
        {
            if (photonView.IsMine)
            {
                //The player is local player. 
                mobileUI.SetActive(true);
                //carController.GetComponent<TinyCarController>().enabled = true;
                
                //TODO - implement better joystick movement using Car Controller
                //transform.GetComponent<MovementController>().enabled = true;
                //transform.GetComponent<MovementController>().joystick.gameObject.SetActive(true);
            }
            else
            {
                //The player is remote player - deactivate controls
                mobileUI.SetActive(false);
                //carController.GetComponent<TinyCarController>().enabled = false;
                
                //transform.GetComponent<MovementController>().enabled = false;
                //transform.GetComponent<MovementController>().joystick.gameObject.SetActive(false);
            }
            SetPlayerName();
        }

        void SetPlayerName()
        {
            // TODO - fix show player name
            if (playerNameText != null)
            {
                if (photonView.IsMine)
                {
                    playerNameText.text = "YOU";
                    playerNameText.color = Color.red;
                }
                else
                {
                    playerNameText.text = photonView.Owner.NickName;

                }

            }

        }

   
    }
}
