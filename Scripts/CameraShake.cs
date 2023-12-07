using UnityEngine;


public class CameraShake : MonoBehaviour
{   
    public float shakeDuration = 0.5f;
    public float shakeAmount = 0.1f;
    private Vector3 originalPos;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    public void Shake()
    {
        LeanTween.value(gameObject, UpdateShake, 0, 1, shakeDuration).setOnComplete(ResetCameraPosition);
    }

    void UpdateShake(float value)
    {
        float xOffset = Random.Range(-shakeAmount, shakeAmount);
        float yOffset = Random.Range(-shakeAmount, shakeAmount);
        Vector3 newPosition = originalPos + new Vector3(xOffset, yOffset, 0);
        transform.localPosition = newPosition;
    }

    void ResetCameraPosition()
    {
        transform.localPosition = originalPos;
    }
}