using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager main;
    static float TimeScale = 1;
    static float FixedTimeScale = 1;
    static float manager_time = 0;
    static float manager_fixed_time = 0;


    private void Awake()
    {
        if (main && main != this)
        {
            Destroy(gameObject);
        }
        if (!main)
        {
            main = this;
        }
    }

    private void Update()
    {
        manager_time += Time.deltaTime * TimeScale;
        manager_fixed_time += Time.fixedDeltaTime * FixedTimeScale;
    }

    public static float deltaTime
    {
        get { return Time.deltaTime * TimeScale; }
    }

    public static float fixedDeltaTime
    {
        get { return Time.fixedDeltaTime * FixedTimeScale; }
    }

    public static float time
    {
        get { return manager_time; }
    }

    public static float fixedTime
    {
        get { return manager_fixed_time; }
    }

    public static void Pause()
    {
        TimeScale = 0f;
        FixedTimeScale = 0f;
    }

    public static void Resume()
    {
        TimeScale = 1f;
        FixedTimeScale = 1f;
    }

    public static void SetTimeScale(float scale)
    {
        TimeScale = scale;
        FixedTimeScale = scale;
    }

    // Reimplementation of WaitForSeconds
    public class WaitForSeconds : CustomYieldInstruction
    {
        float elapsed = 0;
        float timeToWait;

        public WaitForSeconds(float timeToWait_in)
        {
            timeToWait = timeToWait_in;
        }

        public override bool keepWaiting
        {
            get
            {
                bool notDone = elapsed < timeToWait;
                elapsed += TimeManager.deltaTime;
                return notDone;
            }
        }
    }
}
