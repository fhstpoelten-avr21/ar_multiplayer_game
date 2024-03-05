using TMPro;
using UnityEngine;

public class Goal : MonoBehaviour
{
    // Reference to the UI Text elements
    public TextMeshProUGUI goalTextBlue;
    public TextMeshProUGUI goalTextOrange;
    public int currentScoreTeamOrange = 0;
    public int currentScoreTeamBlue = 0;
    // Indicates which team this goal belongs to
    public bool isGoal1;

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
            }
            else
            {
                if (goalTextBlue != null)
                {   
                    currentScoreTeamBlue += 1;
                    goalTextBlue.text = "TEAM BLUE: " + currentScoreTeamBlue;
                }
            }
        }
    }
}
