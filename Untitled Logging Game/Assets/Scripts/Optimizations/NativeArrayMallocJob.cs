using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

struct NativeArrayMallocJob<T> where T: struct, IJob
{
    NativeQueue<T> queueForAllocation;
    Allocator allocator;

    public void Execute()
    {
        queueForAllocation = new NativeQueue<T>(allocator);
    }
}


