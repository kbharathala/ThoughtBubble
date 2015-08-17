using UnityEngine;
using System.Collections;
using System;
using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;


// Change the material when certain poses are made with the Myo armband.
// Vibrate the Myo armband when a fist pose is made.
public class ColorBoxByPose : MonoBehaviour
{
    // Myo game object to connect with.
    // This object must have a ThalmicMyo script attached.
    public GameObject myo = null;

    // Materials to change to when poses are made.
    public Material doubleTapMaterial;

	public ArrayList sphereList = new ArrayList();

    // The pose from the last update. This is used to determine if the pose has changed
    // so that actions are only performed upon making them rather than every frame during
    // which they are active.
    private Pose _lastPose = Pose.Unknown;

	private Vector3 _lastPosition;

	public static int maxScene = Convert.ToInt32(System.IO.File.ReadAllText(@"C:\Users\Jeffrey\Documents\TestProject\config.txt"));
	public static int currScene = maxScene;

	// Information about color buttons being hovered over
	public RaycastHit hitInfo;
	public Color currColor = Color.black;

    // Update is called once per frame.
    void Update ()
    {
		// Access the ThalmicMyo component attached to the Myo game object.
		ThalmicMyo thalmicMyo = myo.GetComponent<ThalmicMyo> ();

		Vector3 origin = Vector3.zero;
		Vector3 direction = transform.position;
		Ray rayProperties = new Ray (origin, direction);

		// Check if the pose has changed since last update.
		// The ThalmicMyo component of a Myo game object has a pose property that is set to the
		// currently detected pose (e.g. Pose.Fist for the user making a fist). If no pose is currently
		// detected, pose will be set to Pose.Rest. If pose detection is unavailable, e.g. because Myo
		// is not on a user's arm, pose will be set to Pose.Unknown.
		if (thalmicMyo.pose != _lastPose) {
			_lastPose = thalmicMyo.pose;

			// Vibrate the Myo armband when a fist is made.
			if (thalmicMyo.pose == Pose.Fist) {
				GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				_lastPosition = transform.position;
				sphere.transform.position = _lastPosition;
				Material materialColored = new Material (Shader.Find ("Unlit/Color"));
				materialColored.color = currColor;
				sphere.gameObject.GetComponent<Renderer>().material = materialColored;
				sphereList.Add (sphere);

				ExtendUnlockAndNotifyUserAction (thalmicMyo);

				// Change material when wave in, wave out or double tap poses are made.
			} else if (thalmicMyo.pose == Pose.WaveIn) {
				if (currScene > 1) {
					LogToFile (sphereList, currScene);
					foreach (GameObject sphere in sphereList) {
						UnityEngine.Object.DestroyImmediate (sphere);
					}
					sphereList = new ArrayList ();
					ArrayList vectorList = ReadFromFile (currScene - 1);
					ArrayList colorList = ReadColorsFromFile(currScene - 1);
					int j = 0;
					foreach (Vector3 vector in vectorList) {
						Material m = new Material (Shader.Find ("Unlit/Color"));
						string colorString = colorList[j];
						float[] colors = RGBAValues(colorString);
						m.color = new Color(colors[0], colors[1], colors[2], colors[3]);
						GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
						sphere.transform.position = vector;
						sphere.gameObject.GetComponent<Renderer>().material = m;
						sphereList.Add (sphere);

						j++;
					}
					currScene--;
				}
				ExtendUnlockAndNotifyUserAction (thalmicMyo);
			} else if (thalmicMyo.pose == Pose.WaveOut) {
				LogToFile (sphereList, currScene);
				foreach (GameObject sphere in sphereList) {
					UnityEngine.Object.DestroyImmediate (sphere);
				}
				sphereList = new ArrayList ();
				if (currScene == maxScene) {
					maxScene++;
					System.IO.File.WriteAllText (@"C:\Users\Jeffrey\Documents\TestProject\config.txt", maxScene.ToString ());
				} else {
					ArrayList vectorList = ReadFromFile (currScene + 1);
					ArrayList colorList = ReadColorsFromFile(currScene + 1);
					int j = 0;
					foreach (Vector3 vector in vectorList) {
						Material m = new Material (Shader.Find ("Unlit/Color"));
						string colorString = colorList[j];
						float[] colors = RGBAValues(colorString);
						m.color = new Color(colors[0], colors[1], colors[2], colors[3]);
						GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
						sphere.transform.position = vector;
						sphere.gameObject.GetComponent<Renderer>().material = m;

						sphereList.Add (sphere);
						j++;
					}
				}
				currScene++;

				ExtendUnlockAndNotifyUserAction (thalmicMyo);
			} else if (thalmicMyo.pose == Pose.DoubleTap && Physics.Raycast (rayProperties, out hitInfo)) {
				if(hitInfo.collider.tag == "color_sel_red") currColor = Color.red;
				else if(hitInfo.collider.tag == "color_sel_green") currColor = Color.green;
				else if(hitInfo.collider.tag == "color_sel_blue") currColor = Color.blue;
				else if(hitInfo.collider.tag == "color_sel_yellow") currColor = Color.yellow;
				else if(hitInfo.collider.tag == "color_sel_black") currColor = Color.black;
				ExtendUnlockAndNotifyUserAction (thalmicMyo);
			}
		} else {
			if (thalmicMyo.pose == Pose.Fist) {

				Material materialColored = new Material (Shader.Find ("Unlit/Color"));
				materialColored.color = currColor;

				GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				Vector3 curr = transform.position;
				sphere.transform.position = curr;
				sphere.gameObject.GetComponent<Renderer>().material = materialColored;
				sphereList.Add (sphere);
				Vector3 avg = new Vector3 ((_lastPosition.x + curr.x) / 2, (_lastPosition.y + curr.y) / 2, (_lastPosition.z + curr.z) / 2);
				GameObject sphere2 = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				Vector3 avg2 = new Vector3 (((_lastPosition.x + curr.x) / 2 + _lastPosition.x) / 2, ((_lastPosition.y + curr.y) / 2 + _lastPosition.y) / 2, (((_lastPosition.z + curr.z) / 2) + _lastPosition.z) / 2);
				GameObject sphere3 = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				sphere2.transform.position = avg;
				sphere2.gameObject.GetComponent<Renderer>().material = materialColored;
				sphere3.transform.position = avg2;
				sphere3.gameObject.GetComponent<Renderer>().material = materialColored;
				Vector3 avg3 = new Vector3 (((_lastPosition.x + curr.x) / 2 + curr.x) / 2, ((_lastPosition.y + curr.y) / 2 + curr.y) / 2, (((_lastPosition.z + curr.z) / 2) + curr.z) / 2);
				GameObject sphere4 = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				sphere4.transform.position = avg3;
				sphere4.gameObject.GetComponent<Renderer>().material = materialColored;
				_lastPosition = curr;
				sphereList.Add (sphere2);
				sphereList.Add (sphere3);
				sphereList.Add (sphere4);
			}
		}
	}

    // Extend the unlock if ThalmcHub's locking policy is standard, and notifies the given myo that a user action was
    // recognized.
    void ExtendUnlockAndNotifyUserAction (ThalmicMyo myo)
    {
        ThalmicHub hub = ThalmicHub.instance;

        if (hub.lockingPolicy == LockingPolicy.Standard) {
            myo.Unlock (UnlockType.Timed);
        }
        myo.NotifyUserAction ();
    }


	void LogToFile(ArrayList gameList, int file){
		string[] coord = new string[gameList.Count*3];
		string[] colors = new string[gameList.Count];
		int i = 0;
		foreach (GameObject sphere in gameList) {
			coord[3*i] =  sphere.transform.position.x.ToString();
			coord[3*i+1] =  sphere.transform.position.y.ToString ();
			coord[3*i+2] =  sphere.transform.position.z.ToString();
			colors[i] = sphere.gameObject.GetComponent<Renderer>().material.color.ToHexStringRGBA();
			i++;
		}
		System.IO.File.WriteAllLines (@"C:\Users\Jeffrey\Documents\TestProject\scene"+file.ToString()+".txt", coord);
		System.IO.File.WriteAllLines (@"C:\Users\Jeffrey\Documents\TestProject\scenec"+file.ToString()+".txt", colors);
	}

	ArrayList ReadFromFile(int file){
		string[] lines = System.IO.File.ReadAllLines (@"C:\Users\Jeffrey\Documents\TestProject\scene"+file.ToString()+".txt");
		ArrayList v = new ArrayList ();
		for (int i = 0; i < lines.Length; i=i+3) {
			v.Add(new Vector3(float.Parse(lines[i]), float.Parse(lines[i+1]), float.Parse(lines[i+2])));
		}
		return v;
	} 
	string[] ReadColorsFromFile(int file){
		string[] lines = System.IO.File.ReadAllLines (@"C:\Users\Jeffrey\Documents\TestProject\scenec"+file.ToString()+".txt");
		return lines;
	}

	float[] RGBAValues(String rgba){
		float[] colors = new float[4];
		colors[0] =  Convert.ToInt32(rgba.Substring (0,2))/255.0f;
		colors[1] =  Convert.ToInt32(rgba.Substring (2, 2))/255.0f;
		colors[2] =  Convert.ToInt32(rgba.Substring (4, 2))/255.0f;
		colors[3] =  Convert.ToInt32(rgba.Substring (6, 2))/255.0f;
		return colors;

	}
}
