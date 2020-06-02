using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

[InternalBufferCapacity(4)]
public struct JobTriangleBuffer : IBufferElementData
{
    // Actual value each buffer element will store.
    public JobTriangle jobTriangle;
}