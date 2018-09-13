using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileSettings
{
    [Header("Data")]
    public int Type = 1;
    public int Cost = 1;
    [Header("Colors")]
    public Color On = new Color(0f, 1f, 0f, 1f);
    public Color Off = new Color(0f, 0.5f, 0f, 1f);
    public Color Hover = new Color(0.8f, 0.8f, 0.8f, 1f);
}

public class TileManager : MonoBehaviour {
    [Header("Color Settings")]
    [SerializeField]
    public TileSettings Settings;
    private Material outMatieral;

    [Header("Neighborhood")]
    [SerializeField]
    public int Column = 0;
    public int Row = 0;
    public List<TileManager> Neighborhood = new List<TileManager>();
    public Manager Overlord = null;

    [Header("Other Settings")]
    [SerializeField]
    protected bool isOn = false;
    public bool getState() { return isOn; }

    private bool isHovering = false;
    
    public void applyColorSettings(TileSettings ts) { Settings = ts; setColor(); }
    public void applyPostitonData(int c, int r) { Column = c; Row = r; }

    private void Awake()
    {   // Initial setting
        outMatieral = GetComponent<MeshRenderer>().material;
        setColor();
    }
    public void OnMouseOver()
    {
        if (!isHovering)
        {
            outMatieral.color *= Settings.Hover;
            isHovering = true;
        }
    }
    public void OnMouseExit()
    {
        setColor();
        isHovering = false;
    }
    public void OnMouseUp()
    {
        if (Overlord == null)
        {
            toggle();
        }
        else
        {
            Overlord.tileClick(this);
        }
        isHovering = false;
    }
    public void setState(bool s)
    {
        isOn = !s;
        toggle();
    }
    public void toggle()
    {
        isOn = !isOn;
        if (Overlord != null)
            if (!sendCost())
                Debug.Log("I, " + this.name + ", fucked upp with cost: " + Settings.Cost + " ind the state: " + isOn);
        setColor();
    }
    private bool sendCost()
    {
        if (isOn)   // Charge for activation
            return Overlord.changeBudget(-Settings.Cost);
        else        // Refund for deactivation
            return Overlord.changeBudget(Settings.Cost);
    }
    public int activeNeighbors()
    {
        int numb = 0;
        foreach(TileManager n in Neighborhood)
        {
            if (n.getState())
                numb++;
        }
        return numb;
    }
    private void setColor()
    {
        if (isOn)
        {   // Swaping to off
            outMatieral.color = Settings.On;
        }
        else
        {   // Swaping to on
            outMatieral.color = Settings.Off;
        }
    }
}
