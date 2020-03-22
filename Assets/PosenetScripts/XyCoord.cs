using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A TO that contains the maximum & minuímum X & Y coordinates for this session.
 */
public class MaxMinCoord {
    public float maxX = -400000.0f;
    public float maxY = -400000.0f;
    public float minX = 400000.0f;
    public float minY = 400000.0f;

    public void handleCoord(Vector2 newCoord) {
        if (newCoord.x < minX) {
            minX = newCoord.x;
        }
        if (newCoord.x > maxX) {
            maxX = newCoord.x;
        }
        if (newCoord.y < minY) {
            minY = newCoord.y;
        }
        if (newCoord.y > maxY) {
            maxY = newCoord.y;
        }
    }

    public override string ToString() {
        return "(minX = " + minX + ", maxX = " + maxX + "), (minY = " + minY + ", maxY = " + maxY + ")";
    }
}

