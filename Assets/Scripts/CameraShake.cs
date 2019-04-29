using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class CameraShake : MonoBehaviour
{
    private float shakeTime = 0.3f;
    private float shakeAmp = 2f;
    private float shakeFreq = 2.0f;
    private float shakeTimer = 0f;
    
    public CinemachineVirtualCamera VirtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;
    
    void Start()
    {
        if (this.VirtualCamera != null)
        {
            this.virtualCameraNoise = this.VirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        }
    }

    public void Shake()
    {
        this.shakeTimer = this.shakeTime;
    }
    
    void Update()
    {
        if (this.VirtualCamera != null && this.virtualCameraNoise != null)
        {
            if (this.shakeTimer > 0)
            {
                this.virtualCameraNoise.m_AmplitudeGain = this.shakeAmp;
                this.virtualCameraNoise.m_FrequencyGain = this.shakeFreq;
                this.shakeTimer -= Time.deltaTime;
            }
            else
            {
                this.virtualCameraNoise.m_AmplitudeGain = 0f;
                this.shakeTimer = 0f;
            }
        }
    }
}