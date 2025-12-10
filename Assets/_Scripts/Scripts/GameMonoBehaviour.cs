using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMonoBehaviour : MonoBehaviour
{
    protected virtual void Awake()
    {
        this.LoadComponent();
    }

    protected virtual void Start()
    {

    }
    protected virtual void Reset()
    {
        this.LoadComponent();
        this.ResetValue();
    }
    protected virtual void ResetValue()
    {

    }
    protected virtual void LoadComponent()
    {

    }

    protected virtual void OnEnable()
    {

    }
}
