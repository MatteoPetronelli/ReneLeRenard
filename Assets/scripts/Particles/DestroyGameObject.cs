using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameObject : MonoBehaviour
{
    private float count = 100;
    void Update()
    {
        count--;
        if (count <= 0)
        {
            Destroy(gameObject);
        }
    }
}
