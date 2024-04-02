using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    private static RaycastHit2D[] rayCastHits = new RaycastHit2D[128];

    public static Vector2 GetMousePositionInWorldSpace()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public static Part GetPartUnderPointer()
    {
        Part topMostPart = null;
        List<Part> partsUnderPointer = new List<Part>();
		
		int hitCount = Physics2D.RaycastNonAlloc(GetMousePositionInWorldSpace(), Vector2.zero, rayCastHits, Mathf.Infinity);

        for (int i = 0; i < hitCount; i++)
        {
			if (rayCastHits[i].collider.gameObject.TryGetComponent(out Part part))
            {
                partsUnderPointer.Add(part);
            }
        }

        topMostPart = GetPartWithHighestSortingOrder(partsUnderPointer);

        return topMostPart;
    }

    public static Part GetPartWithHighestSortingOrder(IEnumerable<Part> parts)
    {
        Part targetPart = null;
        int highestSortingOrder = 0;

        foreach (Part part in parts)
        {
            int currentSortingOrder = part.GetComponent<Renderer>().sortingOrder;

            if (currentSortingOrder >= highestSortingOrder)
            {
                targetPart = part;
                highestSortingOrder = currentSortingOrder;
            }
        }
        return targetPart;
    }

    public static List<Part> GetPartsBetweenPoints(Vector2 p1, Vector2 p2)
	{
		int hitCount = Physics2D.LinecastNonAlloc(p1, p2, rayCastHits);

		List<Part> parts = new List<Part>();

		for (int i = 0; i < hitCount; i++)
        {
			if (rayCastHits[i].collider.gameObject.TryGetComponent(out Part part))
            {
                parts.Add(part);
            }
        }

		return parts;
	}

    
    public static List<Part> GetPartsInRange(float range)
    {
        List <Part> partsInRange = new List <Part>();

		int hitCount = Physics2D.CircleCastNonAlloc(Utilities.GetMousePositionInWorldSpace(), range, Vector2.zero, rayCastHits, Mathf.Infinity);

        for (int i = 0; i < hitCount; i++)
        {
			if (rayCastHits[i].collider.gameObject.TryGetComponent(out Part part))
            {
                partsInRange.Add(part);
            }
        }

        return partsInRange;
    }

    public static Part FindClosestToMouse(List<Part> parts)
    {
        if (parts.Count == 0) { return null; }
        if (parts.Count == 1) { return parts[0]; }

        float smallestDistance = Vector2.Distance(parts[0].transform.position, GetMousePositionInWorldSpace());
        float currentDistance = 0;

        Part closestPart = null;

        foreach(Part o in parts)
        {
            currentDistance = Vector2.Distance(o.transform.position, GetMousePositionInWorldSpace());

            if (currentDistance <= smallestDistance)
            {
                smallestDistance = currentDistance;
                closestPart = o;
            }
        }
        return closestPart;
    }

    public static float GetAngleBetweenLineRenderers(LineRenderer lr1, LineRenderer lr2)
    {
        Vector2 direction1 = GetLineRendererDirection(lr1).normalized;
        Vector2 direction2 = GetLineRendererDirection(lr2).normalized;

        float angle = Vector2.SignedAngle(direction1, direction2);

        // Ensure the angle is in the range [-180, 180)
        if (angle < -180)
        {
            angle += 360;
        }
        else if (angle >= 180)
        {
            angle -= 360;
        }

        return angle;
    }

    public static float GetLineRendererLength(LineRenderer lr)
    {
        float length = 0f;

        if (lr.positionCount == 2)
        {
            length = Vector2.Distance(lr.GetPosition(0), lr.GetPosition(1));
        }
        else
        {
            for(int i = 0; i < lr.positionCount - 1; i++)
            {
                length += Vector2.Distance(lr.GetPosition(i), lr.GetPosition(i+1));
            }
        }
        return length;
    }

    public static float DistanceFromPointToLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        float lineLength = Vector3.Distance(lineStart, lineEnd);

        float t = Mathf.Clamp01(Vector3.Dot(point - lineStart, lineEnd - lineStart) / (lineLength * lineLength));

        Vector3 closestPoint = lineStart + t * (lineEnd - lineStart);

        return Vector3.Distance(point, closestPoint);
    }

    public static Vector3 GetLineRendererDirection(LineRenderer lineRenderer)
    {
        Vector3 startPoint = lineRenderer.GetPosition(0);
        Vector3 endPoint = lineRenderer.GetPosition(lineRenderer.positionCount - 1);

        return endPoint - startPoint;
    }

    public static Vector2 GetClosestPointOnLineRenderer(LineRenderer lr, Vector2 point)
    {
        Vector2 closestPoint = Vector2.zero;
        float closestDistance = Mathf.Infinity;

        if (lr.positionCount == 2)
        {
            Vector2 p1 = lr.GetPosition(0);
            Vector2 p2 = lr.GetPosition(1);

            Vector2 lineDirection = p2 - p1;
            float lineLength = lineDirection.magnitude;
            lineDirection.Normalize();

            Vector2 pointDirection = point - p1;
            float dotProduct = Vector2.Dot(pointDirection, lineDirection);
            dotProduct = Mathf.Clamp(dotProduct, 0f, lineLength);

            closestPoint = p1 + lineDirection * dotProduct;

            return closestPoint;
        }
        else
        {
            for (int i = 0; i < lr.positionCount; i++)
            {
                float distance = Vector2.Distance(lr.GetPosition(i), point);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint = lr.GetPosition(i);
                }
            }
            return closestPoint;
        }
    }

    public static Vector2 FindIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        Vector2 intersection = new Vector2(float.NaN, float.NaN);

        float denominator = (p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);

        // Check if the lines are not parallel
        if (denominator != 0f)
        {
            float t = ((p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x)) / denominator;
            float u = ((p2.x - p1.x) * (p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x)) / denominator;

            // Check if the intersection point is within both line segments
            if (t >= 0f && t <= 1f && u >= 0f && u <= 1f)
            {
                intersection.x = p1.x + t * (p2.x - p1.x);
                intersection.y = p1.y + t * (p2.y - p1.y);
            }
        }

        return intersection;
    }
}
