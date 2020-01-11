using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private static Dictionary<Sound, float> soundTimerDict;

    public enum Sound
    {
        Hit1,
        Hit2,
        Hit3,
        Grapple,
        Toss,
        PlayerMove,
        PlayerDeath,
        EnemyDeath1,
        EnemyDeath2,
        ChesterBark1,
        ChesterBark2,
        ChesterWhine,
        ChesterPant,
        ChesterGrowl,
        MiniCrystalExplosion,
        BigCrystalDeath,
        LEnemy1,
        LEnemy2,
        LEnemy3,
        BEnemy1,
        BEnemy2,
        BEnemy3,
        Fireball,
        P1Grunt,
        P2Grunt,
        Dash,
        HitGrunt1,
        HitGrunt2,
        HitGrunt3,
        HitGrunt4,
        AOEAtk,
        Victory

    }

    // something crackly 
    public SoundAudioClip[] soundAudioArr;

    public static AudioClip punch1, punch2;
    static AudioSource audiosrc;

    private void Awake()
    {
        soundTimerDict = new Dictionary<Sound, float>();
        soundTimerDict[Sound.Hit1] = 0f;
        soundTimerDict[Sound.Hit2] = 0f;
        soundTimerDict[Sound.Hit3] = 0f;

        if (instance && instance != this)
        {
            Destroy(gameObject);
        }
        if (!instance)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        audiosrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            audiosrc.PlayOneShot(punch1);
        }
    }

    public static void PlaySound(Sound sound)
    {
        if (canPlayClip(sound))
        {
            audiosrc.PlayOneShot(GetAudioClip(sound));
        }
    }

    public static void PlaySound(Sound sound, float vol)
    {
        if (canPlayClip(sound))
        {
            audiosrc.PlayOneShot(GetAudioClip(sound), vol);
        }
    }

    // for walking 
    private static bool canPlayClip(Sound sound)
    {
        switch (sound)
        {
            // if any of the fight clips were played, 
            case Sound.Hit1:
            case Sound.Hit2:
            case Sound.Hit3:
                if (soundTimerDict.ContainsKey(sound)) {
                    float lastTimePlayed = soundTimerDict[sound];
                    float interval = 0.2f;

                    if (lastTimePlayed + interval < TimeManager.time)
                    {
                        soundTimerDict[sound] = TimeManager.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    soundTimerDict[sound] = 0f;
                }
                break;
            default:
                return true;
        }
        return false;
    }


    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach(SoundAudioClip s_clip in instance.soundAudioArr)
        {
            if (s_clip.sound == sound)
            {
                return s_clip.audioClip;
            }
        }
        Debug.LogError("Sound " + sound + " not found");
        return null;
    }
}


[System.Serializable]
public class SoundAudioClip
{
    public SoundManager.Sound sound;
    public AudioClip audioClip;
}

