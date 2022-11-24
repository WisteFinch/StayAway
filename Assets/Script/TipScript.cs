using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipScript : MonoBehaviour
{
    public GameObject TargetObj;
    public Color TxtColor = Color.white;
    public float DisplayDistance = 1;
    public float GradientDistance = 1;

    private TextMesh _txt;

    private void Start()
    {
        this._txt = this.GetComponent<TextMesh>();
        this._txt.color = Color.clear;
    }
    void Update()
    {
        float distance = Vector2.Distance(this.TargetObj.transform.position, this.transform.position);
        if(distance >= this.DisplayDistance + this.GradientDistance)
        {
            this._txt.color = Color.clear;
            return;
        }
        if(distance <= this.DisplayDistance)
        {
            this._txt.color = this.TxtColor;
        }
        else
        {
            Color c = this.TxtColor;
            c.a =  1f - (distance - this.DisplayDistance) / this.GradientDistance;
            this._txt.color = c;
        }
    }
}
