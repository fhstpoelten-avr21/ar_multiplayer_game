using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    // Reference to the UI Text elements
    public TextMeshProUGUI goalTextBlue;
    public TextMeshProUGUI goalTextOrange;
    public GameObject uI_winMsgObj; 
    public Text winMsg;
    public int currentScoreTeamOrange = 0;
    public int currentScoreTeamBlue = 0;

    public int winPoints = 5;
    // Indicates which team this goal belongs to
    public bool isGoal1;

    private Vector3 initialBallPosition; // Store the initial position of the ball

    void Start()
    {
        uI_winMsgObj.SetActive(false);

        // Store the initial position of the ball when the game starts
        GameObject ball = GameObject.FindWithTag("ball");
        if (ball != null)
        {
            initialBallPosition = ball.transform.position;
        }
    }


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.name == "Ball")
        {
            Debug.Log("Hit: " + collision.transform.name);

            if (isGoal1)
            {
                if (goalTextOrange != null)
                {
                    currentScoreTeamOrange += 1;
                    goalTextOrange.text = "TEAM ORANGE: " + currentScoreTeamOrange;
                }

                if (currentScoreTeamOrange >= winPoints)
                {
                    // Team orange wins - respawn
                    uI_winMsgObj.SetActive(true); 
                    winMsg.text = "Orange WINS!";
                }
                else
                {
                    // Respawn ball to its initial position
                    ResetBallPosition();
                }
                
            }
            else
            {
                if (goalTextBlue != null)
                {   
                    currentScoreTeamBlue += 1;
                    goalTextBlue.text = "TEAM BLUE: " + currentScoreTeamBlue;
                }
                
                if (currentScoreTeamBlue >= winPoints)
                {
                    // Team blue wins - respawn
                    uI_winMsgObj.SetActive(true); 
                    winMsg.text = "Blue WINS!";
                }
                else
                {
                    // Respawn ball to its initial position
                    ResetBallPosition();
                }
            }
        }
    }

    // Reset the ball to its initial position
    private void ResetBallPosition()
    {
        GameObject ball = GameObject.FindWithTag("ball");
        if (ball != null)
        {
        ball.transform.position = initialBallPosition;
        Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
        if (ballRigidbody != null)
        {
            ballRigidbody.velocity = Vector3.zero;

        }
        }
    }
}
