using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class ScaleController : MonoBehaviour
{

    GameObject battleArena;

    public Slider scaleSlider;


    private void Awake()
    {
        if (gameObject)
        {
            // Access the GameObject to which this script is attached
            battleArena = gameObject;
        }
        else
        {
            battleArena = GameObject.FindGameObjectWithTag("BattleArena");
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        scaleSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }



    public void OnSliderValueChanged(float value)
    {
        if (scaleSlider != null && battleArena)
        {
            battleArena.transform.localScale = Vector3.one / value;

        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
