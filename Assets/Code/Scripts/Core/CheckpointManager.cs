using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour,IReset
{
    public static CheckpointManager instance;
    int checkpointNumber;
    Vector3 _position;
    Vector3 init_position;
    Quaternion _rotation;
    Quaternion init_rotation;
    private void Start() {
        checkpointNumber = -1;
        Transform _player = GameManager.GetGameManager().GetPlayer();
        init_position = _player.position;
        init_rotation = _player.rotation;
        _position = init_position;
        _rotation = init_rotation;
        GameManager.GetGameManager().AddResetObject(this);
    }
    private void Awake() {
        instance = this;
    }
    public void SetCheckpoint(Transform _Checkpoint,int _number)
    {
        if(checkpointNumber > _number) return;
        checkpointNumber = _number;
        _position = _Checkpoint.position;
        _rotation = _Checkpoint.rotation;
    }
    public Vector3 GetCheckpointPos()
    {
        return _position;
    }
    public Quaternion GetCheckpointRot()
    {
        return _rotation;
    }
    public void Reset()
    {
        _position = init_position;
        _rotation = init_rotation;
        checkpointNumber = -1;
    }
}
