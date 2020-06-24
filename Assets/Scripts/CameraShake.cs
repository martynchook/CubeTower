using UnityEngine;

public class CameraShake : MonoBehaviour {
    
    private Transform camTransform;
    private float shakeDur = 1f, shakeAmout = 0.04f, decreaseFactor = 1.5f;
    private Vector3 originPos;

    private void Start () {
        camTransform = GetComponent<Transform>();
        originPos = camTransform.localPosition;
    }

    private void Update()
    {
        if (shakeDur > 0) {
            camTransform.localPosition = originPos + Random.insideUnitSphere * shakeAmout;
            shakeDur -= Time.deltaTime * decreaseFactor;
        }
    }
}
