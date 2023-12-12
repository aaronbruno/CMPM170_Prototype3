using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI : MonoBehaviour
{
    // Start is called before the first frame update
    public Text tex;
    [SerializeField] private Renderer myObject;
    public GameObject steak;
    void Start()
    {
        tex.text = "Don't burn your partner!";
    }

    // Update is called once per frame
    void Update()
    {
        if(myObject.material.color == Color.black)
        {
            tex.text = "You burnt your partner!";
        }
        if(steak.activeSelf == true) 
        {
            tex.text = "You cooked the steak!";
        }
    }
}
