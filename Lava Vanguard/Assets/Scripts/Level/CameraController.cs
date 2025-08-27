using UnityEngine;
using Cinemachine;
using TMPro;
using System;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }
    public static event Action OnCameraUpdated;

    public GameObject player;
    private float currentSpeedY;
    public float cameraSpeedY = 0.3f;
    public float cameraFollowDistance = 5.0f;
    private float remainingDistance = 0f;
    public float initialSpeed = 60f;
    private float totalDistance;
    public EdgeCollider2D[] edges;
    public AnimationCurve speedCurve;
    public CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noiseProfile;
    private CinemachineImpulseSource impulseSource;
    public bool canMove = false;
    public float longPlatformStopPoint = 4.5f; // distance lower from the camera

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentSpeedY = cameraSpeedY;
        if (!Tutorial.Instance.tutorial)
            StartMove();
        noiseProfile = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        impulseSource = virtualCamera.GetComponent<CinemachineImpulseSource>();
        float y = virtualCamera.m_Lens.OrthographicSize;
        float x = y * 16 / 9.0f;
        edges[0].SetPoints(new List<Vector2> { new Vector2(-x, y), new Vector2(x, y) });
        edges[1].SetPoints(new List<Vector2> { new Vector2(-x, -y), new Vector2(x, -y) });
        edges[2].SetPoints(new List<Vector2> { new Vector2(x, -y), new Vector2(x, y) });
        edges[3].SetPoints(new List<Vector2> { new Vector2(-x, -y), new Vector2(-x, y) });

    }

    public void StartMove()
    {
        canMove = true;
    }

    private void LateUpdate()
    {
        Transform cameraTransform = virtualCamera.transform;//Lot of GC!!!
        Vector3 targetPosition = cameraTransform.position;
        if (canMove)
        {
            var lm = LevelManager.Instance;
            if (lm.enteredNext)
            {
                float speed = initialSpeed * speedCurve.Evaluate(remainingDistance / totalDistance);
                targetPosition.y += speed * Time.deltaTime;
                remainingDistance -= speed * Time.deltaTime;
            }
            else
            {
                targetPosition.y += currentSpeedY * Time.deltaTime;
                if (remainingDistance > 0)
                {
                    remainingDistance -= currentSpeedY * Time.deltaTime;
                }
            }

            if (remainingDistance < 0)
            {
                remainingDistance = 0;
                StopCamera();
            }
        }

        cameraTransform.position = targetPosition;
        OnCameraUpdated?.Invoke();
    }

    /// <summary>
    /// Triggers a camera shake effect.
    /// </summary>
    /// <param name="duration">Duration of the shake effect.</param>
    /// <param name="amplitude">Amplitude of the shake (intensity).</param>
    /// <param name="frequency">Frequency of the shake (speed of oscillation).</param>
    public void CameraShake(float duration = 0.25f, float amplitude = 1f, float frequency = 1f)
    {
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
            return;
        }
        if (noiseProfile != null)
        {
            noiseProfile.m_AmplitudeGain = amplitude;
            noiseProfile.m_FrequencyGain = frequency;
            Invoke(nameof(StopShaking), duration);
        }
    }

    /// <summary>
    /// Stops the camera shake effect.
    /// </summary>
    private void StopShaking()
    {
        if (noiseProfile != null)
        {
            noiseProfile.m_AmplitudeGain = 0f;
            noiseProfile.m_FrequencyGain = 0f;
        }
    }

    public void StopCamera()
    {
        currentSpeedY = 0;
    }

    public void ResumeCamera()
    {
        currentSpeedY = cameraSpeedY;
    }

    public void UpdateDistance(Transform transform)
    {
        remainingDistance = transform.position.y - (virtualCamera.transform.position.y - longPlatformStopPoint);
        totalDistance = remainingDistance;
    }

    public bool CameraStopped()
    {
        return currentSpeedY == 0f;
    }

    public void SetCameraSpeed(float speed)
    {
        currentSpeedY = speed;
    }
    public void SetCameraY(float y)
    {
        virtualCamera.transform.position = new Vector3(0, y, virtualCamera.transform.position.z);
    }
}
