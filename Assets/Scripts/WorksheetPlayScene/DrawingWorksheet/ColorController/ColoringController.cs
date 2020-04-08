using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using pl.ayground;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

public class ColoringController : MonoBehaviour
{

    public enum AppState
    {
        COLORING,
        PAGE_PICKER_ENABLED
    }

    public List<Color32> ColorPalette;
    public RawImage Raw_Image;
    public Transform CrayonsContainerContent;
    public ColorSelectionButton ColorButtonPrefab;
    public Camera MyCamera;
    

    private DrawableTextureContainer imageContainer;
    private float imageScale = 1f;
    private Color32 selectedColor;


    private AppState state = AppState.COLORING;
    // Use this for initialization
    GraphicRaycaster raycaster;

    public static ColoringController ColoringController_Instance;

    private void Awake()
    {
        if(ColoringController_Instance == null)
        {
            ColoringController_Instance = this;
        }
        this.raycaster = GetComponent<GraphicRaycaster>();
    }
    void Start()
    {
        InitColorPicker();
        Input.simulateMouseWithTouches = true;
    }

    public void InitColorPicker()
    {
        selectedColor = ColorPalette[0];
        foreach (Color32 c in ColorPalette)
        {
            ColorSelectionButton obj = Instantiate(ColorButtonPrefab, CrayonsContainerContent);
            obj.Init(this, c);
        }
        CrayonsContainerContent.GetComponent<RectTransform>().sizeDelta = new Vector2(100, ColorPalette.Count * 110);
    }

    public void SetColor(Color32 _color)
    {
        selectedColor = _color;
    }

    public void InitWithRandomColoringPage()
    {
        StartCoroutine(setImageFromURl(SetupWorksheets.SetupWorksheetsAnswers_Instance.url));
    }
    #region[Set Image From URL]
    IEnumerator setImageFromURl(string imageurl)
    {
        yield return new WaitForEndOfFrame();
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageurl);
        yield return www.SendWebRequest();

        if (www.isDone)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
            if (tex != null)
            {
                InitWithColoringPage(tex);
                ApploaderController.ApploaderController_Instance.Deactivate_Apploader();
            }
        }
    }
    #endregion

    public void InitWithColoringPage(Texture2D texture)
    {
        imageContainer = new DrawableTextureContainer(texture, true, false);
        Raw_Image.texture = imageContainer.getTexture();
    }
    // Update is called once per frame
    void Update()
    {
        if (state == AppState.COLORING)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Click(Input.mousePosition);
            }
        }
    }

    void Click(Vector3 position)
    {
        //Debug.Log ("On Screen Click: " + position);
        Vector2 localCursor;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(Raw_Image.GetComponent<RectTransform>(), position, MyCamera, out localCursor))
            return;
        else
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            List<RaycastResult> results = new List<RaycastResult>();

            pointerData.position = Input.mousePosition;
            this.raycaster.Raycast(pointerData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.name == "WorksheetImage")
                {
                    //Debug.Log ("Click:" + localCursor);
                    localCursor /= imageScale;
                    //Debug.Log ("    with Scale Adjusted: " + localCursor);
                    localCursor.x += imageContainer.getWidth() / 2;
                    localCursor.y += imageContainer.getHeight() / 2;
                    //Debug.Log ("    with Dimensions adjusted:" + localCursor);
                    setcolor((int)localCursor.x, (int)localCursor.y, selectedColor);
                    DrawingWorksheetEvaluator.DrawingWorksheetEvaluator_instance.MyColoredArea.Add(localCursor);
                    print("77777777777777777777777777777777777777777777" + result.gameObject.name);
                }
                else
                {
                    print("222222222222222222222222222222222222222222" + result.gameObject.name);
                }
            }
        }
    }

    public void setcolor(int x, int y,Color32 Dcolor)
    {
        imageContainer.PaintBucketTool(x, y, Dcolor);
        Raw_Image.texture = imageContainer.getTexture();
    }
}
