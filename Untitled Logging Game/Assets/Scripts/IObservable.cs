using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObservable 
{
    void AddObserver(IObserver observer);

    void Notify();

}
