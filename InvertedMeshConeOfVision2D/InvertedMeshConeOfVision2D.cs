using System;
using MoreMountains.Tools;
using UnityEngine;

public class InvertedMeshConeOfVision2D : MMConeOfVision2D
{
    protected override void DrawMesh()
    {
    	if (!ShouldDrawMesh) return;
        
    	var steps = Mathf.RoundToInt(MeshDensity * VisionAngle);
    	var stepsAngle = VisionAngle / steps;

    	_viewPoints.Clear();

    	for (var i = 0; i <= steps; i++)
    	{
    		var angle = stepsAngle * i + EulerAngles.y - VisionAngle / 2f;
    		_viewCast = RaycastAtAngle(angle);

			if (i > 0)
            {
            	var thresholdExceeded = Mathf.Abs(_oldViewCast.Distance - _viewCast.Distance) > EdgeThreshold;
   
            	if (_oldViewCast.Hit != _viewCast.Hit || (_oldViewCast.Hit && _viewCast.Hit && thresholdExceeded))
            	{
            		var edge = FindMeshEdgePosition(_oldViewCast, _viewCast);
            		if (edge.PointA != Vector3.zero) AddPointAndRadiusPoint(edge.PointA);
            		if (edge.PointB != Vector3.zero) AddPointAndRadiusPoint(edge.PointB);
            	}
            }

            if (_viewCast.Hit) AddPointAndRadiusPoint(_viewCast.Point);
            _oldViewCast = _viewCast;
        }

        var numberOfVertices = _viewPoints.Count;
        if (numberOfVertices < 3)
        {
	        _visionMesh.Clear();
	        return;
        }
        if (numberOfVertices != _numberOfVerticesLastTime)
            Array.Resize(ref _triangles, (numberOfVertices - 2) * 3);

        for (var i = 0; i < numberOfVertices-2; i++)
        {
	        if (i % 2 == 1)
	        {
		        _triangles[i * 3] = i;
		        _triangles[i * 3 + 1] = i + 1;
		        _triangles[i * 3 + 2] = i + 2;
	        }
	        else
	        {
		        _triangles[i * 3] = i + 1;
		        _triangles[i * 3 + 1] = i;
		        _triangles[i * 3 + 2] = i + 2;
	        }
        }

        _visionMesh.Clear();
        _visionMesh.vertices = _viewPoints.ToArray();
        _visionMesh.triangles = _triangles;
        _visionMesh.RecalculateNormals();
        
        _numberOfVerticesLastTime = numberOfVertices;

        void AddPointAndRadiusPoint(Vector3 point)
        {
	        _viewPoints.Add(transform.InverseTransformPoint(point));
	        _viewPoints.Add(VisionRadius * (point - transform.position).MMSetZ(0).normalized);
        }
    }
    
    MeshEdgePosition FindMeshEdgePosition(RaycastData minimumViewCast, RaycastData maximumViewCast)
    {
	    float minAngle = minimumViewCast.Angle;
	    float maxAngle = maximumViewCast.Angle;
	    _minPoint = minimumViewCast.Point;
	    _maxPoint = maximumViewCast.Point;

	    for (int i = 0; i < EdgePrecision; i++)
	    {
		    float angle = (minAngle + maxAngle) / 2;
		    RaycastData newViewCast = RaycastAtAngle(angle);

		    bool thresholdExceeded = Mathf.Abs(minimumViewCast.Distance - newViewCast.Distance) > EdgeThreshold;
		    if (newViewCast.Hit == minimumViewCast.Hit && !thresholdExceeded)
		    {
			    minAngle = angle;
			    _minPoint = newViewCast.Point;
		    }
		    else
		    {
			    maxAngle = angle;
			    _maxPoint = newViewCast.Point;
		    }
	    }

	    return new MeshEdgePosition(_minPoint, _maxPoint);
    }
    
    RaycastData RaycastAtAngle(float angle)
    {
	    _direction = MMMaths.DirectionFromAngle2D(angle, 0f);

	    _raycastAtAngleHit2D = Physics2D.Raycast(this.transform.position, _direction, VisionRadius, ObstacleMask);

	    if (_raycastAtAngleHit2D)
	    {
		    _returnRaycastData.Hit = true;
		    _returnRaycastData.Point = _raycastAtAngleHit2D.point;
		    _returnRaycastData.Distance = _raycastAtAngleHit2D.distance;
		    _returnRaycastData.Angle = angle;
	    }
	    else
	    {
		    _returnRaycastData.Hit = false;
		    _returnRaycastData.Point = this.transform.position + _direction * VisionRadius;
		    _returnRaycastData.Distance = VisionRadius;
		    _returnRaycastData.Angle = angle;
	    }

	    return _returnRaycastData;
    }
}
