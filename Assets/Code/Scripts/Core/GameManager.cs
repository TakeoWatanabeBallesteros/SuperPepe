using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private List<IReset> resetObjects;
    private int checkpointReference;
    private Vector3 currentCheckpointPos;
    private Quaternion currentCheckpointRot;
    private Transform player;
    
    // Input
    private PlayerInput playerInput;
    public delegate void GameOverEvent(int hasLifes);
    public static event GameOverEvent OnGameOverEvent;
    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public static GameManager GetGameManager()
    {
        if(instance == null)
        {
            instance = new GameObject("GameManager").AddComponent<GameManager>();
            instance.resetObjects = new List<IReset>();
        }
        return instance;
    }
    private void OnSceneLoaded(Scene a,LoadSceneMode b)
    {
        resetObjects = new List<IReset>();
        playerInput = FindObjectOfType<PlayerInput>();
    }
    public void SetPlayer(Transform _player)
    {
        this.player = _player;
    }
    public Transform GetPlayer()
    {
        return player != null? player : FindObjectOfType<PlayerFSM>().transform;
    }
    public void GameOver(int hasLifes)
    {
        OnGameOverEvent?.Invoke(hasLifes);
        
    }
    public void ResetGame()
    {
        foreach (var other in resetObjects)
        {
            other.Reset();
        }
    }

    public void ChangeActionMap(string map)
    {
        playerInput.currentActionMap = playerInput.actions.FindActionMap(map);
    }

    public void AddResetObject(IReset obj)
    {
        resetObjects.Add(obj);
    }
    
    public void RemoveResetObject(IReset obj)
    {
        resetObjects.Remove(obj);
    }
}
