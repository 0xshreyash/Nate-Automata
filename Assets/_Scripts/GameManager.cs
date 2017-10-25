﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System;
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
    public bool IsMouseCursorHidden = false;
    public bool IsDontDestroyBetweenScenes = false;

    [Header("Overlay panel to show win/lose")]
    public Image FadeImage;
    public Text PanelText;
    public float Alpha;
    public int FadeMax = 60;
    public float FadeSpeed = 0.10f;

    [Header("Particles")]
    public GameObject ParticleSystemPrefab;

    public GameObject Fireworks = (GameObject)Resources.Load("Fireworks");


    private float t = 0;

    void Awake() {
        if (instance == null) {
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
        //do more

        Instantiate(Fireworks, Camera.main.transform);

    }

    public void LoseGame()
    {
        Debug.Log("LOSE");
        IsGameOver = true;
        PanelText.text = "HACKING FAILURE";
    }

    //Called from enemies and player, set in the editor as a UnityEvent
    public void UnitsChanged()
    {
        Debug.Log("Units in scene changed");

        StartCoroutine(checkUnitChange());
    }

    IEnumerator checkUnitChange()
    {
        yield return 0; //TODO: waits one frame, set optional delay?

        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        var player = GameObject.FindGameObjectWithTag("Player");

        if (enemies.Length == 0)
            WinGame();
        if (player == null)
            LoseGame();
    }

    public void LoadLevel(string name)
    {
        Application.LoadLevel(name.Trim());
        SoundManager.GetInstance().PlayConfirmSound();

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

}
