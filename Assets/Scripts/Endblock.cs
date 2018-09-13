using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Endblock : MonoBehaviour {
    public TextMesh text;
    public MeshRenderer mesh;

    public void setText(string txt)
    {
        text.text = txt;
    }
    public void setMesh(Vector3 rotation, Vector3 scale, Color c)
    {   // Spatialy mesh
        mesh.gameObject.transform.Rotate(rotation);
        mesh.gameObject.transform.localScale = scale;
        // Rotate text
        text.gameObject.transform.Rotate(rotation);
        // Color
        mesh.material.color = c;
    }
}
