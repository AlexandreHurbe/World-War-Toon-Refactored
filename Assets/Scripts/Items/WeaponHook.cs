using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHook : MonoBehaviour
{
    public Transform leftHandIK;
    [HideInInspector]
    public float lastFired;

    private ParticleSystem[] particles;
    private AudioSource audioSource;

    public Transform slider;
    public AnimationCurve sliderCurve;
    public float multiplier = 1;
    public Vector3 startPos;
    public float sliderSpeed = 5;

    private float sliderT;
    public bool isShooting;
    bool initSliderLerp;

    public void Init()
    {
        if (slider != null)
        {
            startPos = slider.localPosition;

        }

        GameObject go = new GameObject();
        go.name = "audio holder";
        go.transform.parent = this.transform;
        audioSource = go.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1;

        particles = transform.GetComponentsInChildren<ParticleSystem>();
    }

    public void Shoot()
    {
        isShooting = true;

        if (particles != null)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].Play();
            }

        }
    }

    private void Update()
    {
        if (isShooting)
        {
            if (!initSliderLerp)
            {
                initSliderLerp = true;
                sliderT = 0;
            }

            sliderT += Time.deltaTime * sliderSpeed;
            if (sliderT > 1)
            {
                sliderT = 1;
                initSliderLerp = false;
                isShooting = false;
            }

            float targetZ = sliderCurve.Evaluate(sliderT) * multiplier;
            Vector3 tp = startPos;
            tp.z -= targetZ;
            slider.transform.localPosition = tp;
        }
    }
}
