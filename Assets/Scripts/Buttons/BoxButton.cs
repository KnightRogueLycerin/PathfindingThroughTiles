using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BoxButton : MonoBehaviour {
    // Sound
    public AudioClip TiggerSFX;
    public AudioSource source;
    // Material
    public Material Material;
    public float MaterialDarkening = 0.5f;
    private Material offMat;
    private MeshRenderer mr;
    // Particle system
    public float SwitchRate = 5f;
    public float ParticleDarkening = 0.8f;
    public bool isOn = false;
    private ParticleSystem pse;
    
    private void Awake()
    {
        pse = gameObject.GetComponent<ParticleSystem>();
        var main = pse.main;
            main.startColor = new Color(
            Material.color.r * ParticleDarkening, 
            Material.color.g * ParticleDarkening, 
            Material.color.b * ParticleDarkening, 
            1f);
        offMat = new Material(Material);
        offMat.color = new Color(
            Material.color.r * MaterialDarkening,
            Material.color.g * MaterialDarkening,
            Material.color.b * MaterialDarkening,
            1f);
        mr = gameObject.GetComponent<MeshRenderer>();
    }

    // Use this for initialization
    void Start () {
        if (isOn)
        {   // Start on
            pse.Play();
            mr.material = Material;
        }
        else
        {   // Start off
            mr.material = offMat;
        }
    }
    public void OnMouseUp()
    {
        toggle();
    }
    private void toggle()
    {
        if (isOn)
        {   // Swaping to off
            pse.Stop();
            mr.material = offMat;
        }
        else
        {   // Swaping to on
            pse.Play();
            mr.material = Material;
        }
        isOn = !isOn;
        source.PlayOneShot(TiggerSFX);
    }
}
