using System;
using System.Collections;
using Cinemachine;
using UnityEngine;

/// <summary>
/// 收到剧情信号后播放拔剑：动画 / 音效 / 特效 / 镜头特写。
/// Timeline Signal Receiver 可直接绑 <see cref="OnStorySignal()"/>。
/// </summary>
public class BossDrawSword : MonoBehaviour
{
    [Header("剧情信号")]
    [Tooltip("为空则接受任意信号；否则只响应匹配的 id")]
    [SerializeField] string requiredSignalId = "Boss00_DrawSword";
    [SerializeField] bool listenGameEvents = true;
    [SerializeField] bool playOnce = true;
    [SerializeField] bool allowReplay;

    [Header("动画")]
    [SerializeField] Animator animator;
    [SerializeField] string drawSwordTrigger = "DrawSword";

    [Header("音效")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip drawSwordSfx;
    [SerializeField] [Range(0f, 1f)] float sfxVolume = 1f;
    [SerializeField] bool playSfxOnStart = true;

    [Header("特效")]
    [SerializeField] GameObject drawSwordVfxPrefab;
    [SerializeField] ParticleSystem drawSwordParticles;
    [SerializeField] Transform vfxPoint;
    [SerializeField] float vfxLifetime = 3f;
    [SerializeField] bool playVfxOnStart = true;

    [Header("特写")]
    [SerializeField] CinemachineVirtualCamera closeUpCamera;
    [SerializeField] Transform closeUpTarget;
    [SerializeField] bool createRuntimeCloseUp = true;
    [SerializeField] float closeUpOrthoSize = 3f;
    [SerializeField] int closeUpPriority = 100;
    [SerializeField] float closeUpDuration = 1.5f;

    [Header("时序")]
    [SerializeField] float playDuration = 1.5f;

    public bool IsPlaying { get; private set; }
    public bool HasPlayed { get; private set; }

    public event Action Completed;

    CinemachineVirtualCamera _runtimeVcam;
    int _storedPriority;
    bool _storedActive;
    bool _closeUpActive;
    Coroutine _playRoutine;

    void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (vfxPoint == null)
            vfxPoint = transform;
        if (closeUpTarget == null)
            closeUpTarget = transform;
    }

    void OnEnable()
    {
        if (listenGameEvents)
            GameEvents.Instance.OnStorySignal += HandleStorySignal;
    }

    void OnDisable()
    {
        if (GameEvents.HasInstance)
            GameEvents.Instance.OnStorySignal -= HandleStorySignal;

        RestoreCloseUp();
        IsPlaying = false;
    }

    void HandleStorySignal(string signalId)
    {
        if (!string.IsNullOrEmpty(requiredSignalId) && signalId != requiredSignalId)
            return;

        Play();
    }

    /// <summary>Timeline Signal Receiver 无参入口。</summary>
    public void OnStorySignal()
    {
        Play();
    }

    public void OnStorySignal(string signalId)
    {
        HandleStorySignal(signalId);
    }

    public void Play()
    {
        if (!isActiveAndEnabled)
            return;
        if (IsPlaying)
            return;
        if (playOnce && HasPlayed && !allowReplay)
            return;

        if (_playRoutine != null)
            StopCoroutine(_playRoutine);
        _playRoutine = StartCoroutine(PlayRoutine());
    }

    public void AnimEvent_PlaySlashVfx()
    {
        SpawnVfx();
    }

    public void AnimEvent_PlaySlashSfx()
    {
        PlaySfx();
    }

    IEnumerator PlayRoutine()
    {
        IsPlaying = true;

        if (animator != null && !string.IsNullOrEmpty(drawSwordTrigger))
            animator.SetTrigger(drawSwordTrigger);

        if (playSfxOnStart)
            PlaySfx();
        if (playVfxOnStart)
            SpawnVfx();

        StartCloseUp();

        float wait = Mathf.Max(playDuration, closeUpDuration);
        if (wait > 0f)
            yield return new WaitForSeconds(wait);
        else
            yield return null;

        RestoreCloseUp();
        HasPlayed = true;
        IsPlaying = false;
        _playRoutine = null;
        Completed?.Invoke();
    }

    void PlaySfx()
    {
        if (drawSwordSfx == null)
            return;

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.PlayOneShot(drawSwordSfx, sfxVolume);
    }

    void SpawnVfx()
    {
        Transform point = vfxPoint != null ? vfxPoint : transform;

        if (drawSwordParticles != null)
        {
            drawSwordParticles.transform.position = point.position;
            drawSwordParticles.Clear(true);
            drawSwordParticles.Play(true);
        }

        if (drawSwordVfxPrefab == null)
            return;

        GameObject vfx = Instantiate(drawSwordVfxPrefab, point.position, point.rotation);
        if (vfxLifetime > 0f)
            Destroy(vfx, vfxLifetime);
    }

    void StartCloseUp()
    {
        CinemachineVirtualCamera vcam = closeUpCamera;
        if (vcam == null && createRuntimeCloseUp)
        {
            GameObject go = new GameObject("Boss00_CloseUpCam");
            go.transform.SetPositionAndRotation(
                closeUpTarget.position + new Vector3(0f, 0f, -10f),
                Quaternion.identity
            );
            vcam = go.AddComponent<CinemachineVirtualCamera>();
            vcam.Follow = closeUpTarget;
            vcam.LookAt = closeUpTarget;
            vcam.m_Lens.OrthographicSize = closeUpOrthoSize;
            _runtimeVcam = vcam;
        }

        if (vcam == null)
            return;

        _storedPriority = vcam.Priority;
        _storedActive = vcam.gameObject.activeSelf;
        vcam.gameObject.SetActive(true);
        vcam.Priority = closeUpPriority;
        if (closeUpTarget != null)
        {
            vcam.Follow = closeUpTarget;
            vcam.LookAt = closeUpTarget;
        }
        _closeUpActive = true;
    }

    void RestoreCloseUp()
    {
        if (!_closeUpActive && _runtimeVcam == null)
            return;

        if (_runtimeVcam != null)
        {
            Destroy(_runtimeVcam.gameObject);
            _runtimeVcam = null;
        }
        else if (closeUpCamera != null)
        {
            closeUpCamera.Priority = _storedPriority;
            closeUpCamera.gameObject.SetActive(_storedActive);
        }

        _closeUpActive = false;
    }
}
