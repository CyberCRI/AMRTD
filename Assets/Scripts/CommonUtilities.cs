﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonUtilities
{
    public static void fillArrayFromRoot(Transform root, ref Transform[] array)
    {
        array = new Transform[root.childCount];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = root.GetChild(i);
        }
    }

    public static void fillArrayFromRoot<T>(Transform root, ref T[] array)
    {
        array = new T[root.childCount];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = root.GetChild(i).GetComponent<T>();
        }
    }

    public static float getEffectMaxDuration(ParticleSystem ps)
    {
        return ps.main.duration + ps.main.startLifetime.constant;
    }
    
    public static GameObject createDebugObject(Vector3 position, string _name, float scale = 1f)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = scale * Vector3.one;
        sphere.name = _name;
        GameObject.Destroy(sphere.GetComponent<Collider>());
        return sphere;
    }
}
