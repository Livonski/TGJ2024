using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceSlider
{
    private float maxSpeed = 5f;

    public SurfaceSlider (float maxSpeed)
    {
        this.maxSpeed = maxSpeed;
    }

    public Vector2 projectVector(Vector2 groundNormal, Vector2 inputDirection)
    {
        Vector2 forward = Vector2.Perpendicular(groundNormal) * Mathf.Sign(Vector2.Dot(groundNormal, Vector2.down));
        float inputProjected = Vector2.Dot(inputDirection, forward);
        Vector2 targetVelocity = forward * inputProjected * maxSpeed;
        return targetVelocity;
    }
}
