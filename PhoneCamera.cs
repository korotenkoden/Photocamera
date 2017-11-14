using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneCamera : MonoBehaviour {

	public bool camAvailable;
	WebCamTexture backCam;
	Texture defaultBackground;
	public RawImage background;
	public AspectRatioFitter fit;

	public Button buttonStartCamAndClose;
	public Button buttonGalleryAndTakePhoto;
	public Button buttonClose;
	public Button buttonReTakePhoto;
	public Button buttonCloseViewImage;

	public Text startStopText;
	public Text takePhotoAndGallery;

	public GameObject _ScrollView;
	public GameObject content;
	public GameObject prefabImage;

	public Texture2D snapshot;
	public List<Texture2D> savedSnapshots = new List<Texture2D>();

	int currentCamIndex = 0;
	public int _IdCounter = 0;

	public GameObject forViewImage;
	public Image ViewImage;

	// Use this for initialization

	public void StartCam_Clicked()
	{
		_ScrollView.gameObject.SetActive(false);
		buttonGalleryAndTakePhoto.gameObject.SetActive (true);
		snapshot = null;

		defaultBackground = background.texture;
		if (backCam != null) {
			background.texture = null;
			backCam.Stop ();
			backCam = null;
			startStopText.text = "Камера";
			takePhotoAndGallery.text = "Галерея";
			camAvailable = false;
		} else {
			WebCamDevice device = WebCamTexture.devices [currentCamIndex];
			backCam = new WebCamTexture (device.name);
			background.texture = backCam;
			backCam.Play ();
			camAvailable = true;
			startStopText.text = "Назад";
			takePhotoAndGallery.text = "Cфотографировать";
		}
	}

	public void TakePhoto_Clicked()
	{
		//проверка - если была нажата кнопка Галерея а не Сфотографировать - значит отправляем в Галерею
		if (takePhotoAndGallery.text != "Cфотографировать") {
			GalleryClick ();
			return;
		}

		if (backCam != null) {
			snapshot = new Texture2D(backCam.width, backCam.height);
			snapshot.SetPixels(backCam.GetPixels());
			snapshot.Apply();
			backCam.Stop ();
			camAvailable = false;

			buttonStartCamAndClose.gameObject.SetActive(false);
			buttonGalleryAndTakePhoto.gameObject.SetActive(false);
			buttonClose.gameObject.SetActive(true);
			buttonReTakePhoto.gameObject.SetActive(true);

		}
	}

	public void ReTakePhoto_Clicked()
	{
		snapshot = null;

		buttonStartCamAndClose.gameObject.SetActive(true);
		buttonGalleryAndTakePhoto.gameObject.SetActive(true);
		buttonClose.gameObject.SetActive(false);
		buttonReTakePhoto.gameObject.SetActive(false);

		WebCamDevice device = WebCamTexture.devices [currentCamIndex];
		backCam = new WebCamTexture (device.name);
		background.texture = backCam;
		backCam.Play ();
		camAvailable = true;
	}

	public void CloseCapturedPhoto_Clicked()
	{
		if (snapshot != null) {

			snapshot.name = _IdCounter.ToString ();

			savedSnapshots.Add (snapshot);

			_IdCounter++;

			buttonClose.gameObject.SetActive(false);
			buttonReTakePhoto.gameObject.SetActive(false);

			background.texture = null;
			backCam.Stop ();
			backCam = null;
		}

		_ScrollView.gameObject.SetActive(true);
		GalleryClick ();
		snapshot = null;

		startStopText.text = "Камера";
		buttonStartCamAndClose.gameObject.SetActive(true);
	}

	public void GalleryClick()
	{
		buttonGalleryAndTakePhoto.gameObject.SetActive (false);
		_ScrollView.gameObject.SetActive(true);

		if (snapshot != null) {
			GameObject image = (GameObject)Instantiate(prefabImage);
			image.name = "Image" + snapshot.name;
			Rect rec = new Rect(0, 0, snapshot.width, snapshot.height);
			image.GetComponent<Image> ().sprite = Sprite.Create(snapshot,rec,new Vector2(0.1f,0.1f));
			image.transform.parent = content.transform;
		}
	}

	public void Image_Click(Image img)
	{
		if (img.sprite != null) {
			forViewImage.SetActive(true);
			ViewImage.sprite = img.sprite;
			background.gameObject.SetActive(false);
			_ScrollView.gameObject.SetActive(false);
			buttonStartCamAndClose.gameObject.SetActive(false);
			buttonCloseViewImage.gameObject.SetActive (true);
			buttonStartCamAndClose.gameObject.SetActive (false);
		}
	}

	public void CloseImageView_Click()
	{
		buttonCloseViewImage.gameObject.SetActive (false);
		forViewImage.SetActive(false);
		background.gameObject.SetActive(true);
		_ScrollView.gameObject.SetActive(true);
		startStopText.text = "Камера";
		buttonStartCamAndClose.gameObject.SetActive(true);
	}

	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (camAvailable) {
			float ratio = (float)backCam.width / (float)backCam.height;
			fit.aspectRatio = ratio;

			float scaleY = backCam.videoVerticallyMirrored ? -1F: 1f;
			background.rectTransform.localScale = new Vector3 (1f, scaleY, 1f);

			int orient = -backCam.videoRotationAngle;
 			background.rectTransform.localEulerAngles = new Vector3 (0, 0, orient);
		}
	}
}
