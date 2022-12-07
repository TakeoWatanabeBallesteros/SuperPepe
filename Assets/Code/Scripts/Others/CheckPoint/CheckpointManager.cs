using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance;
    int checkpointNumber;
    Vector3 position;
    Vector3 forstPosition;
    Quaternion rotation;
    Quaternion firstRotation;
    private void Start() {
        checkpointNumber = -1;
        Transform player = GameManager.GetGameManager().GetPlayer();
        forstPosition = player.position;
        firstRotation = player.rotation;
        position = forstPosition;
        rotation = firstRotation;
    }
    private void Awake() {
        instance = this;
    }
    public void SetCheckpoint(Transform checkpoint,int number)
    {
        if(checkpointNumber > number) return;
        checkpointNumber = number;
        position = checkpoint.position;
        rotation = checkpoint.rotation;
    }

    public Vector3 GetCheckPointPosition() => position;

    public Quaternion GetCheckPointRotation() => rotation;
}
