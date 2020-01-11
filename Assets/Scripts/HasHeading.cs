using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasHeading : MonoBehaviour
{
    Vector2 current_pos = Vector2.up;
    Vector2 prev_pos = Vector2.zero;
    Vector2 prev_diff_pos = Vector2.zero;

    Vector2 cur_heading;

    void Start()
    {
        cur_heading = Vector2.zero;
    }

    void Update()
    {
        // Vector2 new_pos = VUtils.Vec3ToVec2(transform.position);
        // if (new_pos != current_pos)
        // {
        //     prev_pos = current_pos;
        //     prev_diff_pos = current_pos;
        //     current_pos = new_pos;
        // }
        // else
        // {
        //     prev_diff_pos = current_pos;
        // }
    }

    public Vector2 GetHeading()
    {
        return cur_heading;
    }

    public void SetHeading(Vector2 heading_in)
    {
        cur_heading = heading_in;
    }

    public float GetSpeed()
    {
        Vector2 new_pos = VUtils.Vec3ToVec2(transform.position);
        if (new_pos != current_pos)
        {
            prev_pos = current_pos;
            prev_diff_pos = current_pos;
            current_pos = new_pos;
        }
        else
        {
            prev_diff_pos = current_pos;
        }

        return (current_pos - prev_diff_pos).magnitude;
    }
}
