using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour
{
    [Space(10)]
    [Header("Variables")]
    [SerializeField] private float ropeSegLen;
    [SerializeField] private int segmentLength;
    [SerializeField] private float lineWidth;

    [Space(10)]
    [Header("References")]
    [SerializeField] private LineRenderer lineRenderer;

    [HideInInspector] public Vector2 StartPos;
    [HideInInspector] public Vector2 EndPos;

    private List<WireSegment> wireSegments = new List<WireSegment>();
    private Vector2 gravity = new Vector2(0f, -15f);

    [HideInInspector] public GameObject startPointConnectionPoint;
    [HideInInspector] public GameObject endPointConnectionPoint;


    public struct WireSegment
    {
        public Vector2 currentPos;
        public Vector2 previousPos;

        public WireSegment(Vector2 pos)
        {
            this.currentPos = pos;
            this.previousPos = pos;
        }
    }

    public void Disable()
    {
        lineRenderer.enabled = false;
    }

    public void Enable()
    {
        lineRenderer.enabled = true;
    }

    public void Initialize()
    {
        StartPos = Utilities.GetMousePositionInWorldSpace();

        lineRenderer.startWidth = lineRenderer.endWidth = lineWidth;

        for (int i = 0; i < segmentLength; i++)
        {
            this.wireSegments.Add(new WireSegment(StartPos));
            StartPos.y -= ropeSegLen;
        }
    }

    public void SetPositions(Vector3[] positions)
    {
        this.lineRenderer.positionCount = positions.Length;
        this.lineRenderer.SetPositions(positions);

        for (int i = 0; i < segmentLength; i++)
        {
            this.wireSegments.Add(new WireSegment(positions[i]));
        }

        StartPos = positions[0];
        EndPos = positions[positions.Length - 1];
    }

    public Vector3[] GetPositions()
    {
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positions);
        return positions;
    }

    void LateUpdate()
    {
        if (lineRenderer.enabled)
        {
            DrawRope();
        }
    }

    private void FixedUpdate()
    {
        if (startPointConnectionPoint && endPointConnectionPoint)
        {
            StartPos = startPointConnectionPoint.transform.position;
            EndPos = endPointConnectionPoint.transform.position;
        }

        if (lineRenderer.enabled)
        {
            CalculatePositions();
        }
    }

    private void CalculatePositions()
    {
        for (int i = 1; i < this.segmentLength; i++)
        {
            WireSegment firstSegment = this.wireSegments[i];
            Vector2 velocity = firstSegment.currentPos - firstSegment.previousPos;
            firstSegment.previousPos = firstSegment.currentPos;

            // Apply damping
            firstSegment.currentPos += velocity * 0.65f;

            firstSegment.currentPos += gravity * Time.fixedDeltaTime;
            wireSegments[i] = firstSegment;
        }

        for (int i = 0; i < 10; i++)
        {
            ApplyConstraints();
        }
    }

    private void ApplyConstraints()
    {
        WireSegment firstSegment = wireSegments[0];
        firstSegment.currentPos = StartPos;
        wireSegments[0] = firstSegment;

        for (int i = 0; i < this.segmentLength - 1; i++)
        {
            WireSegment firstSeg = wireSegments[i];
            WireSegment secondSeg = wireSegments[i + 1];

            float dist = (firstSeg.currentPos - secondSeg.currentPos).magnitude;
            float error = Mathf.Abs(dist - ropeSegLen);
            Vector2 changeDir = Vector2.zero;

            if (dist > ropeSegLen)
            {
                changeDir = (firstSeg.currentPos - secondSeg.currentPos).normalized;
            } else if (dist < ropeSegLen)
            {
                changeDir = (secondSeg.currentPos - firstSeg.currentPos).normalized;
            }

            Vector2 changeAmount = changeDir * error;
            if (i != 0)
            {
                firstSeg.currentPos -= changeAmount * 0.5f;
                wireSegments[i] = firstSeg;
                secondSeg.currentPos += changeAmount * 0.5f;
                wireSegments[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.currentPos += changeAmount;
                wireSegments[i + 1] = secondSeg;
            }
        }

        WireSegment lastSegment = wireSegments[wireSegments.Count - 1];
        lastSegment.currentPos = EndPos;
        wireSegments[wireSegments.Count - 1] = lastSegment;
    }

    private void DrawRope()
    {
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        Vector3[] ropePositions = new Vector3[this.segmentLength];
        for (int i = 0; i < segmentLength; i++)
        {
            ropePositions[i] = wireSegments[i].currentPos;
        }

        lineRenderer.positionCount = ropePositions.Length;
        lineRenderer.SetPositions(ropePositions);
    }

    public void PreCalculatePositionsAndSetActive()
    {
        for (int j = 1; j < 10; j++)
        {
            for (int i = 1; i < segmentLength && i < wireSegments.Count; i++)
            {
                WireSegment firstSegment = wireSegments[i];
                Vector2 velocity = firstSegment.currentPos - firstSegment.previousPos;
                firstSegment.previousPos = firstSegment.currentPos;

                // Apply damping
                firstSegment.currentPos += velocity * 0.65f;

                firstSegment.currentPos += gravity * Time.fixedDeltaTime;
                wireSegments[i] = firstSegment;
            }

            for (int i = 0; i < 10; i++)
            {
                ApplyConstraints();
            }
        }

        gameObject.SetActive(true);
    }
}