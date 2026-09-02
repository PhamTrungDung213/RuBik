using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RubikFunction : MonoBehaviour
{
    public GameObject rubik;
    public RubikLayerRotate layerRotate;
    public Button solveButton;
    public Button shuffleButton;
    public float solveSpeed;
    public float shuffleSpeed;

    // Luu vi tri ban dau cua tung cubelet
    struct CubeletState { public Transform t; public Vector3 pos; public Quaternion rot; }
    List<CubeletState> initialStates = new List<CubeletState>();

    // Lich su cac nuoc xoay (truc, lop, goc)
    // Thuat toan giai: Reverse Sequence - chay nguoc lai tung nuoc, dao dau goc
    // Dua tren ly thuyet nhom: neu chuoi bien doi la G = M1*M2*...*Mn
    // thi G^-1 = Mn^-1 * ... * M2^-1 * M1^-1 (moi Mi^-1 = xoay nguoc goc)
    struct Move { public Vector3 axis; public float layer; public float angle; }
    List<Move> history = new List<Move>();

    void Start()
    {
        foreach (Transform child in rubik.transform)
            initialStates.Add(new CubeletState { t = child, pos = child.localPosition, rot = child.localRotation });

        layerRotate.OnMoveComplete += (axis, layer, angle) =>
            history.Add(new Move { axis = axis, layer = layer, angle = angle });

        solveButton.onClick.AddListener(OnSolve);
        shuffleButton.onClick.AddListener(OnShuffle);
    }

    void OnShuffle()
    {
        if (layerRotate.rotating) return;
        StartCoroutine(ShuffleRoutine());
    }

    void OnSolve()
    {
        if (layerRotate.rotating) return;
        StartCoroutine(SolveRoutine());
    }

    IEnumerator ShuffleRoutine()
    {
        InstantReset();
        history.Clear();
        yield return null;

        Vector3[] axes = { Vector3.up, Vector3.right, Vector3.forward };
        float[] outerLayers = { -1f, 1f };
        float[] angles = { 90f, -90f };

        Vector3 lastAxis = Vector3.zero;

        float savedSpeed = layerRotate.speed;
        layerRotate.speed = shuffleSpeed;

        for (int i = 0; i < 20; i++)
        {
            Vector3 axis;
            do { axis = axes[Random.Range(0, axes.Length)]; }
            while (axis == lastAxis);
            lastAxis = axis;

            float layer = outerLayers[Random.Range(0, outerLayers.Length)];
            float angle = angles[Random.Range(0, angles.Length)];

            yield return StartCoroutine(layerRotate.RotateLayer(axis, layer, angle));
        }

        layerRotate.speed = savedSpeed;
    }

    IEnumerator SolveRoutine()
    {
        if (history.Count == 0)
        {
            InstantReset();
            yield break;
        }

        // Thuat toan Reverse Sequence:
        // Duyet lich su tu cuoi ve dau, moi nuoc dao dau goc (-angle)
        // Dam bao dung 100% voi moi trang thai dat duoc bang cac nuoc trong game
        float savedSpeed = layerRotate.speed;
        layerRotate.speed = solveSpeed;
        layerRotate.recordMoves = false;

        for (int i = history.Count - 1; i >= 0; i--)
        {
            Move m = history[i];
            yield return StartCoroutine(layerRotate.RotateLayer(m.axis, m.layer, -m.angle));
        }

        layerRotate.speed = savedSpeed;
        layerRotate.recordMoves = true;
        history.Clear();
    }

    void InstantReset()
    {
        foreach (var st in initialStates)
        {
            st.t.localPosition = st.pos;
            st.t.localRotation = st.rot;
        }
    }
}
