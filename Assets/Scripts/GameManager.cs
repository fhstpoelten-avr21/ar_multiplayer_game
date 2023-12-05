using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @Author - Malek Morad
 * The GameManager is responsible for:
 * 1. keeping all global variables and flags in one file
 * 2. keeping and setting configs
 * 3. tracking the current state of the game
 * 
 */
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool gameFieldIsSet = false;
    public bool gameHasStarted = false;
    public bool gameHasFinished = false;
    public int playerJoined = 0;

    // Called once, before start
    // makes sure it acts as a singleton (only one Instance)
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
