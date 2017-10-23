﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCreator : MonoBehaviour
{
    public GameObject BallPrefab;
    public Vector2 XBounds;
    public Vector2 YBounds;
    public int NumOfBalls;

    // Use this for initialization
    void Start () {
        for (int i = 0; i < NumOfBalls; i++)
        {
            Vector3 pos = new Vector3(Random.Range(XBounds.x, XBounds.y), Random.Range(YBounds.x, YBounds.y));
            Instantiate(BallPrefab, pos, transform.rotation, transform);
        }
    }
}