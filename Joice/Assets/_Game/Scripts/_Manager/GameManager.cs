using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] internal bool LOCKED = false;
    internal Transform m_Spawn;

    void Awake()
    {
        if (Instance is null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    IEnumerator Start()
    {
        if (!SceneHandler.SceneLoaded(0))
            SceneHandler.SceneAdd(0);

        yield return new WaitUntil(() => SceneHandler.SceneLoaded(0));

        m_Spawn = GameObject.FindGameObjectWithTag("Spawn").transform;
        if (m_Spawn != null)
            PlayerManager.Instance.SetTransform(m_Spawn);

        AudioManager.Instance.PlayMainMusic();

        yield return null;
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
            OptionsOverlay();
    }

    void OptionsOverlay()
    {
        LOCKED = !LOCKED;

        //Pause
        if (LOCKED)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (!SceneHandler.SceneLoaded("Options"))
                SceneHandler.SceneAdd("Options");

            AudioManager.Instance.Play(AudioManager.Instance.m_AudioInfo.Panel.clips[0]);
        }
        //Continue
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (SceneHandler.SceneLoaded("Options"))
                SceneHandler.SceneRemove("Options");

            AudioManager.Instance.Play(AudioManager.Instance.m_AudioInfo.Panel.clips[1]);
        }
    }
}