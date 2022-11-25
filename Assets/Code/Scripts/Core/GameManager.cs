using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private List<IReset> resetObjects;
    private int checkpointReference;
    private Vector3 currentCheckpointPos;
    private Quaternion currentCheckpointRot;
    private Transform player;
    
    //settings
    public float sensibility { get; private set; } = 10;
    
    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
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
    public void SetPlayer(Transform _player)
    {
        this.player = _player;
        currentCheckpointPos = player.position;
        currentCheckpointRot = player.rotation;
        checkpointReference = 0;
        resetObjects = new List<IReset>();
    }
    public void GameOver()
    {
        //
    }
    public void ResetGame()
    {
        foreach (var other in resetObjects)
        {
            other.Reset();
        }
    }

    public void AddResetObject(IReset obj)
    {
        resetObjects.Add(obj);
    }
    
    public void RemoveResetObject(IReset obj)
    {
        resetObjects.Remove(obj);
    }
    
    public void SetCheckpoint(Transform checkpoint, int reference)
    {
        if(reference < checkpointReference) return;
        currentCheckpointPos = checkpoint.position;
        currentCheckpointRot = checkpoint.rotation;
        checkpointReference = reference;
    }

    public void SetSensibility(float sensibility)
    {
        this.sensibility = sensibility;
    }
}
