using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UnityEngine.Events;

public class CameraZoomAndMove : MonoBehaviour
{
    public static CameraZoomAndMove Instance { get; private set; }
    public CinemachineVirtualCamera vcam;   
    public Vector3 targetPosition;           
    public float targetSize = 3f;           
    public float duration = 2f;
    public bool isMoving = false;
    public Canvas UIWorldCanvas;

    private float originalSize;
    private Vector3 originalPosition;

    private bool focused = false;
    private void Awake()
    {
        Instance = this;
    }
    public void ZoomAndMove(TweenCallback callback = null)
    {
        isMoving = true;
        focused = true;
        originalSize = vcam.m_Lens.OrthographicSize;
        originalPosition = vcam.transform.position;

        Time.timeScale = 0f;
        targetPosition = PlayerManager.Instance.playerView.transform.position + new Vector3(4, 0, 0);
        targetPosition.z = -10;

        DOTween.To(() => vcam.m_Lens.OrthographicSize,
                   x => vcam.m_Lens.OrthographicSize = x,
                   targetSize,
                   duration)
               .SetEase(Ease.OutQuad).SetUpdate(true);


        vcam.transform.DOMove(targetPosition, duration)
                      .SetEase(Ease.OutQuad).SetUpdate(true).onComplete += () =>
                      {
                          isMoving = false;
                      } + callback;
    }

    public void ResetCamera()
    {
        isMoving = true;

        DOTween.To(() => vcam.m_Lens.OrthographicSize,
                   x => vcam.m_Lens.OrthographicSize = x,
                   originalSize,
                   duration)
               .SetEase(Ease.OutQuad).SetUpdate(true);

        vcam.transform.DOMove(originalPosition, duration)
                      .SetEase(Ease.OutQuad).SetUpdate(true).onComplete += () =>
                      {
                          isMoving = false;
                          focused = false;
                          Time.timeScale = 1f;
                      };
    }
    public void ResetCameraInstantly()
    {
        isMoving = false;
        focused = false;
        Time.timeScale = 1;
        vcam.m_Lens.OrthographicSize = originalSize;
    }
    private void Update()
    {
        if (!focused)
            UIWorldCanvas.transform.position = new Vector3(0, vcam.transform.position.y, 0);
    }
}
