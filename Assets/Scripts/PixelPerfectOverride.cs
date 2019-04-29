using Cinemachine;
using UnityEngine;
using UnityEngine.U2D;
using System.Reflection;

[RequireComponent(typeof(PixelPerfectCamera), typeof(CinemachineBrain))]
class PixelPerfectOverride : MonoBehaviour
{
    CinemachineBrain cb;
    object ppc;
    FieldInfo orthoInfo;

    void Start()
    {
        this.cb = GetComponent<CinemachineBrain>();
        this.ppc = typeof(PixelPerfectCamera).GetField("m_Internal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GetComponent<PixelPerfectCamera>());
        this.orthoInfo = this.ppc.GetType().GetField("orthoSize", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    void LateUpdate()
    {
        var cam = this.cb.ActiveVirtualCamera as CinemachineVirtualCamera;
        if (cam)
        {
            cam.m_Lens.OrthographicSize = (float) this.orthoInfo.GetValue(this.ppc);
        }
    }
}