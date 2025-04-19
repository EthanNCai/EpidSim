using UnityEngine;

public class ShakeDebugger:MonoBehaviour{
    public CameraShake cameraShake;
    public void Shake(){
        cameraShake.TriggerShake();
    }
}