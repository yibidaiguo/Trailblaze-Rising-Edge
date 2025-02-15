﻿using JKFrame;
using System.Collections.Generic;
using UnityEngine;

public class ClientRTTInfo : SingletonMono<ClientRTTInfo>
{
    public int rttMs { get; private set; }
    private Queue<int> rttTimeQueue;
    [SerializeField] private int calFrames = 100;
    private int totalMs;
    protected override void Awake()
    {
        base.Awake();
        rttTimeQueue = new Queue<int>(calFrames);
    }
    private void OnDisable()
    {
        rttTimeQueue.Clear();
        totalMs = 0;
        rttMs = 0;
    }
    private void FixedUpdate()
    {
        if (NetManager.Instance == null) return;

        if (NetManager.Instance.IsConnectedClient)
        {
            if (rttTimeQueue.Count >= 100)
            {
                totalMs -= rttTimeQueue.Dequeue();
            }
            int currentRtt = (int)NetManager.Instance.unityTransport.GetCurrentRtt(NetManager.ServerClientId);
            rttTimeQueue.Enqueue(currentRtt);
            totalMs += currentRtt;
            rttMs = totalMs / rttTimeQueue.Count;
        }
    }
}
