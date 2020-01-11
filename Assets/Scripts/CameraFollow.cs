using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float Smoothing = 0.1f;
    public float MaxZoom = 6f;
    public float MinZoom = 9f;
    public float MaxPlayerDispersion = 8f;
    public float MinPlayerDispersion = 3f;
    public bool UseEdgeDetection = false;
    public float Margin;
    public bool mainGame;
    public static CameraFollow main;

    public static float horizExtent, vertExtent;

    bool manual_panning = false;
    Vector3 follow_offset = Vector3.zero;

    private void Awake() {
        if (main && main != this) {
            Destroy(gameObject);
        }
        if (main == null) {
            main = this;
        }
    }

    private void Start()
    {
        vertExtent = Camera.main.GetComponent<Camera>().orthographicSize;
        horizExtent = Mathf.RoundToInt(vertExtent * Screen.width / Screen.height);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            ShakeScreen(2, 2f);
        }
        if (mainGame)
        {
            if (PlayerManager.GetNumPlayers() > 0)
            {
                if (!manual_panning) {
                    FollowTarget(PlayerManager.GetCenterOfMass());
                    EncompassPlayers();
                }
            }
        }
    }

    //Press "B" to test!!!
    public static void ShakeScreen(float duration, float intensity) {
        main.StartCoroutine(ShakeScreenCoroutine(duration, intensity));
    }

    static IEnumerator ShakeScreenCoroutine(float duration, float intensity) {
        float timer = 0;
        // int num_shakes = 0;
        // float shake_freq = 0;
        while (timer < duration) {
            timer += TimeManager.deltaTime;
            main.follow_offset = VUtils.Vec2ToVec3(Random.insideUnitCircle * intensity);
            yield return null;
        }
        main.follow_offset = Vector3.zero;
    }

    void FollowTarget(Vector3 target) {
        Vector3 new_position = Vector3.Lerp(transform.position, target + follow_offset, TimeManager.deltaTime * Smoothing);
        new_position.z = transform.position.z;
        if (UseEdgeDetection) {
            transform.position = KeepInBounds(new_position);
        } else {
            transform.position = new_position;
        }
    }

    void EncompassPlayers() {
        float player_dispersion = PlayerManager.GetDispersion();
        Collider2D col = Physics2D.OverlapPoint(VUtils.Vec3ToVec2(transform.position), LayerMask.GetMask("CameraInfo"));
        float min_z = MaxZoom;
        float max_z = MinZoom;
        if (col) {
            ForcedZoomComponent f = col.gameObject.GetComponent<ForcedZoomComponent>();
            min_z = f.ZoomMin;
            max_z = f.ZoomMax;
        }
        player_dispersion = Mathf.Clamp(player_dispersion, MinPlayerDispersion, MaxPlayerDispersion);
        player_dispersion -= MinPlayerDispersion;
        player_dispersion /= MaxPlayerDispersion - MinPlayerDispersion;
        float zoom = min_z + player_dispersion * (max_z - min_z);
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, zoom, TimeManager.deltaTime);
    }

    public static void PanCamera(Vector3 location, float time) {
        main.StartCoroutine(main.PanCameraCoroutine(location + main.follow_offset, time));
    }

    IEnumerator PanCameraCoroutine(Vector3 location, float time) {
        manual_panning = true;
        float timer = 0;
        float current_zoom = Camera.main.orthographicSize;
        while (timer < time) {
            timer += TimeManager.deltaTime;
            FollowTarget(location);
            if (timer < 0.5f) {
                Camera.main.orthographicSize = Mathf.Lerp(current_zoom, main.MaxZoom, timer / 0.5f);
            }
            yield return null;
        }

        manual_panning = false;
    }

    Vector3 KeepInBounds(Vector3 new_position) {

        Vector3 clamped_position = new_position;
        if (new_position.y > transform.position.y) {
            Vector3 check_position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1, 0));
            check_position.y -= Margin;
            
            if (!Physics2D.OverlapPoint(VUtils.Vec3ToVec2(check_position), LayerMask.GetMask("Nav"))) {
                clamped_position.y = transform.position.y;
            }
        } else {
            Vector3 check_position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0, 0));
            check_position.y += Margin;
            
            if (!Physics2D.OverlapPoint(VUtils.Vec3ToVec2(check_position), LayerMask.GetMask("Nav"))) {
                clamped_position.y = transform.position.y;
            }
        }
        
        if (new_position.x > transform.position.x) {
            Vector3 check_position = Camera.main.ViewportToWorldPoint(new Vector3(1, 0.5f, 0));
            check_position.x -= Margin;
            
            if (!Physics2D.OverlapPoint(VUtils.Vec3ToVec2(check_position), LayerMask.GetMask("Nav"))) {
                clamped_position.x = transform.position.x;
            }
        } else {
            Vector3 check_position = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, 0));
            check_position.x += Margin;
            
            if (!Physics2D.OverlapPoint(VUtils.Vec3ToVec2(check_position), LayerMask.GetMask("Nav"))) {
                clamped_position.x = transform.position.x;
            }
        }

        return clamped_position;
    }
}
