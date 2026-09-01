using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubikLayerRotate : MonoBehaviour
{
    public GameObject rubik;
    public float speed = 300f;

    [HideInInspector] public bool rotating = false;
    [HideInInspector] public bool recordMoves = true;
    public System.Action<Vector3, float, float> OnMoveComplete;

    Vector2 mouseStart;
    Transform hitCube;
    Vector3 localHitNormal;

    // Live drag
    bool dragStarted;
    Vector3 liveDragAxis;
    float liveDragLayer;
    List<Transform> liveDragCubes;
    float liveDragAngle;
    Vector2 liveDragScreenDir;
    float liveDragSign;
    const float DRAG_TO_90 = 150f; // pixels cần kéo để xoay 90°

    void Update()
    {
        if (rotating) return;
        HandleKeyboard();
        HandleMouse();
    }

    void HandleKeyboard()
    {
        bool shift = Input.GetKey(KeyCode.LeftShift);
        float dir = shift ? -1f : 1f;

        if (Input.GetKeyDown(KeyCode.U)) StartCoroutine(Rotate(Vector3.up,       1f,  90f * dir));
        if (Input.GetKeyDown(KeyCode.D)) StartCoroutine(Rotate(Vector3.up,      -1f, -90f * dir));
        if (Input.GetKeyDown(KeyCode.R)) StartCoroutine(Rotate(Vector3.right,    1f,  90f * dir));
        if (Input.GetKeyDown(KeyCode.L)) StartCoroutine(Rotate(Vector3.right,   -1f, -90f * dir));
        if (Input.GetKeyDown(KeyCode.F)) StartCoroutine(Rotate(Vector3.forward, -1f,  90f * dir));
        if (Input.GetKeyDown(KeyCode.B)) StartCoroutine(Rotate(Vector3.forward,  1f, -90f * dir));
        // Lop giua: M (giua L-R), E (giua U-D), S (giua F-B)
        if (Input.GetKeyDown(KeyCode.M)) StartCoroutine(Rotate(Vector3.right,   0f, -90f * dir));
        if (Input.GetKeyDown(KeyCode.E)) StartCoroutine(Rotate(Vector3.up,      0f, -90f * dir));
        if (Input.GetKeyDown(KeyCode.S)) StartCoroutine(Rotate(Vector3.forward, 0f,  90f * dir));
    }

    void HandleMouse()
    {
        // Nhan chuot: xac dinh mat va vi tri bat dau
        if (Input.GetMouseButtonDown(0) && !rotating)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.IsChildOf(rubik.transform))
            {
                hitCube        = hit.transform;
                localHitNormal = SnapToAxis(rubik.transform.InverseTransformDirection(hit.normal));
                mouseStart     = Input.mousePosition;
                dragStarted    = false;
                liveDragAngle  = 0f;
                liveDragCubes  = null;
            }
        }

        // Giu chuot: keo xoay truc tiep
        if (hitCube != null && Input.GetMouseButton(0) && !rotating)
        {
            Vector2 drag = (Vector2)Input.mousePosition - mouseStart;

            // Sau khi keo > 10px moi xac dinh truc xoay
            if (!dragStarted && drag.magnitude > 10f)
            {
                SetupLiveDrag(drag);
                dragStarted = true;
            }

            if (dragStarted)
                ApplyLiveDrag(drag);
        }

        // Tha chuot: snap ve 90° gan nhat
        if (Input.GetMouseButtonUp(0) && hitCube != null)
        {
            if (dragStarted && liveDragCubes != null)
                StartCoroutine(SnapDrag());
            else
                hitCube = null;

            dragStarted = false;
        }
    }

    void SetupLiveDrag(Vector2 drag)
    {
        Vector3 A1, A2;
        GetAxes(localHitNormal, out A1, out A2);

        Camera cam = Camera.main;
        Vector3 center = rubik.transform.position;
        Vector2 sA1 = (Vector2)cam.WorldToScreenPoint(center + rubik.transform.TransformDirection(A1))
                    - (Vector2)cam.WorldToScreenPoint(center);
        Vector2 sA2 = (Vector2)cam.WorldToScreenPoint(center + rubik.transform.TransformDirection(A2))
                    - (Vector2)cam.WorldToScreenPoint(center);

        float dotA1 = Vector2.Dot(drag.normalized, sA1.normalized);
        float dotA2 = Vector2.Dot(drag.normalized, sA2.normalized);
        Vector3 localCubePos = rubik.transform.InverseTransformPoint(hitCube.position);

        if (Mathf.Abs(dotA1) >= Mathf.Abs(dotA2))
        {
            liveDragAxis    = localHitNormal;
            liveDragLayer   = Mathf.Round(Vector3.Dot(localCubePos, localHitNormal));
            liveDragScreenDir = sA1.normalized;
            liveDragSign    = -Mathf.Sign(dotA1);
        }
        else
        {
            liveDragAxis    = A1;
            liveDragLayer   = Mathf.Round(Vector3.Dot(localCubePos, A1));
            liveDragScreenDir = sA2.normalized;
            liveDragSign    = -Mathf.Sign(dotA2);
        }

        liveDragCubes = GetLayer(liveDragAxis, liveDragLayer);
        liveDragAngle = 0f;
    }

    void ApplyLiveDrag(Vector2 drag)
    {
        float projected   = Vector2.Dot(drag, liveDragScreenDir);
        float targetAngle = Mathf.Clamp(projected / DRAG_TO_90 * 90f * liveDragSign, -90f, 90f);
        float delta       = targetAngle - liveDragAngle;

        if (Mathf.Abs(delta) > 0.01f)
        {
            Vector3 worldAxis = rubik.transform.TransformDirection(liveDragAxis);
            foreach (var t in liveDragCubes)
                t.RotateAround(rubik.transform.position, worldAxis, delta);
            liveDragAngle = targetAngle;
        }
    }

    IEnumerator SnapDrag()
    {
        rotating = true;

        float snapped   = Mathf.Round(liveDragAngle / 90f) * 90f;
        float remaining = snapped - liveDragAngle;
        Vector3 worldAxis = rubik.transform.TransformDirection(liveDragAxis);

        // Animate snap den vi tri 90° gan nhat
        float done = 0f, abs = Mathf.Abs(remaining);
        while (done < abs - 0.01f)
        {
            float step = Mathf.Min(speed * Time.deltaTime, abs - done);
            foreach (var t in liveDragCubes)
                t.RotateAround(rubik.transform.position, worldAxis, Mathf.Sign(remaining) * step);
            done += step;
            yield return null;
        }

        // Snap vi tri chinh xac
        foreach (var t in liveDragCubes)
        {
            Vector3 lp = rubik.transform.InverseTransformPoint(t.position);
            t.position = rubik.transform.TransformPoint(
                new Vector3(Mathf.Round(lp.x), Mathf.Round(lp.y), Mathf.Round(lp.z)));
            Vector3 le = t.localEulerAngles;
            t.localEulerAngles = new Vector3(
                Mathf.Round(le.x / 90f) * 90f,
                Mathf.Round(le.y / 90f) * 90f,
                Mathf.Round(le.z / 90f) * 90f);
        }

        if (Mathf.Abs(snapped) > 1f && recordMoves)
            OnMoveComplete?.Invoke(liveDragAxis, liveDragLayer, snapped);

        hitCube       = null;
        liveDragCubes = null;
        rotating      = false;
    }

    public IEnumerator RotateLayer(Vector3 localAxis, float layerValue, float totalAngle)
    {
        yield return StartCoroutine(Rotate(localAxis, layerValue, totalAngle));
    }

    IEnumerator Rotate(Vector3 localAxis, float layerValue, float totalAngle)
    {
        rotating = true;

        List<Transform> layer = GetLayer(localAxis, layerValue);
        Vector3 worldCenter = rubik.transform.position;
        Vector3 worldAxis   = rubik.transform.TransformDirection(localAxis);

        float done = 0;
        while (done < Mathf.Abs(totalAngle))
        {
            float step = Mathf.Min(speed * Time.deltaTime, Mathf.Abs(totalAngle) - done);
            foreach (var t in layer)
                t.RotateAround(worldCenter, worldAxis, Mathf.Sign(totalAngle) * step);
            done += step;
            yield return null;
        }

        foreach (var t in layer)
        {
            Vector3 lp = rubik.transform.InverseTransformPoint(t.position);
            t.position = rubik.transform.TransformPoint(
                new Vector3(Mathf.Round(lp.x), Mathf.Round(lp.y), Mathf.Round(lp.z)));
            Vector3 le = t.localEulerAngles;
            t.localEulerAngles = new Vector3(
                Mathf.Round(le.x / 90f) * 90f,
                Mathf.Round(le.y / 90f) * 90f,
                Mathf.Round(le.z / 90f) * 90f);
        }

        if (recordMoves) OnMoveComplete?.Invoke(localAxis, layerValue, totalAngle);
        rotating = false;
    }

    List<Transform> GetLayer(Vector3 localAxis, float value)
    {
        var result = new List<Transform>();
        foreach (Transform child in rubik.transform)
        {
            Vector3 lp = rubik.transform.InverseTransformPoint(child.position);
            if (Mathf.Abs(Vector3.Dot(lp, localAxis) - value) < 0.4f)
                result.Add(child);
        }
        return result;
    }

    void GetAxes(Vector3 normal, out Vector3 right, out Vector3 up)
    {
        normal = normal.normalized;
        if (Mathf.Abs(Vector3.Dot(normal, Vector3.up)) < 0.9f)
        {
            right = Vector3.Cross(Vector3.up, normal).normalized;
            up    = Vector3.Cross(normal, right).normalized;
        }
        else
        {
            right = Vector3.Cross(Vector3.forward, normal).normalized;
            up    = Vector3.Cross(normal, right).normalized;
        }
    }

    Vector3 SnapToAxis(Vector3 v)
    {
        float ax = Mathf.Abs(v.x), ay = Mathf.Abs(v.y), az = Mathf.Abs(v.z);
        if (ax >= ay && ax >= az) return new Vector3(Mathf.Sign(v.x), 0, 0);
        if (ay >= ax && ay >= az) return new Vector3(0, Mathf.Sign(v.y), 0);
        return new Vector3(0, 0, Mathf.Sign(v.z));
    }
}
