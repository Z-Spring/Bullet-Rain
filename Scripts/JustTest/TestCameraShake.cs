using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class TestCameraShake : MonoBehaviour
{
    // 0.22  0.11
    public float shakeDuration = 0.5f; // 抖动持续时间
    public float shakeMagnitude = 0.2f; // 抖动幅度

    private Vector3 originalPosition;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // 按下空格键触发摄像机抖动
        {
            StartCoroutine(ShakeCamera());
        }
    }

    IEnumerator ShakeCamera()
    {
        originalPosition = transform.localPosition;
        float elapsed = 0.0f;
        float noiseSeed = Random.Range(0f, 100f);

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            // 使用Perlin噪声计算新的x和y坐标
            float x = Mathf.PerlinNoise(noiseSeed, elapsed * 3f) * shakeMagnitude * 2 - shakeMagnitude;
            float y = Mathf.PerlinNoise(elapsed * 3f, noiseSeed) * shakeMagnitude * 2 - shakeMagnitude;

            // 更新摄像机的位置
            transform.localPosition = new Vector3(x, y, originalPosition.z);

            yield return null;
        }

        // 抖动结束，恢复原始位置
        transform.localPosition = originalPosition;
    }
}