using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaShader : MonoBehaviour
{
    Material mat;
    private void Start() {
        mat = GetComponent<Material>();
        mat.shader.GetPropertyDefaultVectorValue(2).Set(transform.lossyScale.x,transform.lossyScale.y,0,0);
    }
}
