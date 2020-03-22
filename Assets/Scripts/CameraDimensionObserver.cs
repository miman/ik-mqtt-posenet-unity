using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface CameraDimensionObserver
{
    void cameraDimensionsChanged(Vector3 viewBottomLeft, Vector3 viewTopRight, float viewWidth, float viewHeight);
}
