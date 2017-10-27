using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

/// <summary>
/// Game Manager is a global object and won't be destroyed ever, it is implemented as a singleton
/// so that there is one game manager per game.
/// </summary>
public class GameManager : MonoBehaviour {
    
    static GameManager instance;
    public static GameManager GetInstance() {
        if (instance == null) {
            var k = GameObject.Find("GameManager").GetComponent<GameManager>();
            instance = k;

            if (instance.IsDebug)
                Debug.Log("GameManager accessed first time from GetInstance");

        }
        return instance;
    }

    public bool IsDebug = true;

    [Header("Common Core Attributes")]
    public bool IsWon = false;
    public bool IsGameOver = false;
    public bool IsMouseCursorHidden = true;
    public bool IsDontDestroyBetweenScenes = false;
    public Canvas menu;

    [Header("Overlay panel to show win/lose")]
    public Image FadeImage;
    public Text PanelText;
    public float Alpha;
    public int FadeMax = 60;
    public float FadeSpeed = 0.10f;

    [Header("Particles")]
    public GameObject ParticleSystemPrefab;
    public GameObject FireworksPrefab;

    [Header("Random background fireworks to be deleted")]
    private GameObject fireworksLeft;
    private GameObject fireworksRight;

    [Header("Fireworks to be played upon successful hacking")]
    private GameObject fireworksVictory;

    [Header("Keep track of whether any enemy has died")]
    // For playing enemy death SFX
    private int numEnemies;

    [Header("Check if the player has completed the tutorial or not")]
    public static int CurrentLevel = 0;
    public static int CurrentProgression = -1;

    private float t = 0;

    void Awake() {
        //Cursor.visible = false;
        if (instance == null)
        {
            instance = this;
            Cursor.visible = !IsMouseCursorHidden;
            Cursor.lockState = CursorLockMode.Confined;
            //don't destroy between scenes
            if (IsDontDestroyBetweenScenes)
                DontDestroyOnLoad(gameObject);

            Debug.Log("GameManager initialized");
        }
    }


    public void Update() {
        // If the level is either over or the player has won then show this screen.
        if (IsWon || IsGameOver) {
            
            t += FadeSpeed * Time.deltaTime;
            // Lerp to smoothen stuff out.
            Alpha = Mathf.Lerp(Alpha, FadeMax, t);

            float aValue = Alpha / 255;
            FadeImage.color = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b, aValue);
            PanelText.color = new Color(PanelText.color.r, PanelText.color.g, PanelText.color.b, aValue);

            //TODO improve
            if (Alpha >= FadeMax - 5)
                LoadLevel("levelselect");
        }
        
    }

    public void WinGame()
    {
        IsWon = true;
        PanelText.text = "HACKING COMPLETE";
        // Update the current progression if the level won is higher than the current progression
        CurrentProgression = CurrentLevel > CurrentProgression 
            ? CurrentLevel 
            : CurrentProgression++;
        SoundManager.GetInstance().PlayStageWonSound();
        //do more
        
        // TODO How to re-activate the fireworks???
        Camera.main.gameObject.SetActive(true);
        
        //fireworksLeft = Utils.InstantiateSafe(FireworksPrefab,new Vector3(-50f,-50f,-50f));
        fireworksVictory = Utils.InstantiateSafe(FireworksPrefab,new Vector3(0,0,0));

        fireworksVictory.transform.parent = Camera.main.transform;

        // TODO How to re-activate the fireworks???
        /* fireworksVictory.transform.gameObject.SetActive(true);
         fireworksVictory.GetComponent<Renderer>().enabled = true;
         foreach (Transform child in fireworksVictory.transform)
             child.gameObject.SetActive(true);
         */

    }

    public void LoseGame()
    {
        Debug.Log("LOSE");
        CurrentLevel = 0;
        IsGameOver = true;
        PanelText.text = "HACKING FAILURE";
        SoundManager.GetInstance().PlayStageOverSound();
    }

    //Called from enemies and player, set in the editor as a UnityEvent
    public void UnitsChanged()
    {
        Debug.Log("Units in scene changed");
        StartCoroutine(checkUnitChange());
    }

    IEnumerator checkUnitChange()
    {
        yield return 0; // TODO: waits one frame, set optional delay?

        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        var player = GameObject.FindGameObjectWithTag("Player");
        
        // If an enemy has died, play the enemy death SFX
        if (numEnemies > enemies.Length)
        {
            SoundManager.GetInstance().PlayObjectDestroyedSound();
            numEnemies--;
        }

        Debug.Log("Enemies length: " + enemies.Length);
        if (enemies.Length == 0)
            WinGame();
        if (player == null)
            LoseGame();
    }

    public void LoadLevel(string levelName)
    {
        Debug.Log("Current Progression: " + CurrentProgression);
        var levelchar = levelName.Substring(levelName.Length - 2, 2);
        //Debug.Log(levelchar);
        int level;
        if (levelName.Equals("tutorialmenu") || levelName.Equals("storymenu"))
        {
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;
        }
        var isInt = int.TryParse(levelchar, out level);
        //Debug.Log(isInt);
        
        // Assume that when last two char is an int => level
        // Thus requires that utility scenes does not have numeric char at the end of the scane's name
        if (isInt)
        {
            // The player is trying to progress into another level
            // The boundary here (+1) means the player is able to challenge the level 
            // that has +1 more his/her progression
            if (CurrentProgression+1 < level)
            {
                // Current progression is less that requested level
                // Do not go load level
                return;
            }
            // If current progression is more than requested level
            // Allow to pass
            // Set pointer to the current level
            CurrentLevel = level;
            Debug.Log("Challenging: " + CurrentLevel);
        }
       
        //Cursor.visible = false;
        //Application.LoadLevel(name.Trim());
        SceneManager.LoadScene(levelName.Trim());
        
        /* TODO Not implementing this for now, the sound is only 
            played for a very short fraction of a second.
            Need to find some other way to play this sound */
        //SoundManager.GetInstance().PlayConfirmSound();

        //reset global values
        IsWon = false;
        IsGameOver = false;
        t = 0;
    }

    public void EndGame()
    {
        #if UNITY_EDITOR
                Debug.Break();
        #else
        Application.Quit();
        #endif
    }

    public void SpawnParticles(Vector3 position)
    {
        Utils.InstantiateSafe(ParticleSystemPrefab, position);
        
    }

    public void HideTutorialMenu()
    {
        //Debug.Log("Trying to make the canvas invisible");
        menu.enabled = false;
    }
    /*
    private void OnDestroy()
    {
        menu.SetActive(true);
    }
    */
}
