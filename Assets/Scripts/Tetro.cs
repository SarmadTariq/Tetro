using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tetro : MonoBehaviour
{
	public int points = 0;
	private Vector3 rotationPoint = new Vector3(0.5f,0.5f,0);
	private static int height = 15;
	private static int width = 14;
	private float speed = 1;
	private float time = 0;
	public bool paused = false;
	List<int> completeLines;
	Coroutine coroutine;
	public Transform[,] grid = new Transform[width, height];
	public GameObject pauseMenu;

	Vector2Int[] directions = {
		new Vector2Int(1, 0),
		new Vector2Int(-1, 0),
		new Vector2Int(0, 1),
		new Vector2Int(0, -1)
	};

	private void Update() {
		time += Time.deltaTime/speed;
		if(time > 10)
		{
			speed += 0.1f;
			Time.timeScale = speed;
			time = 0;
		}
		time += Time.deltaTime;
        
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(!paused)
			{
				pauseOn();
			}
			else
			{
				pauseOff();
			}
		}
	}
	public void pauseOn()
	{
		//pauseMenu.SetActive(true);
		Time.timeScale = 0;
		paused = true;
	}
	public void pauseOff()
	{
		//pauseMenu.SetActive(false);
		Time.timeScale = speed;
		paused = false;
	}

	public void AddToGrid(Transform obj) {
		completeLines = new List<int>();
		foreach (Transform children in obj)
		{
			int X = Mathf.RoundToInt(width+children.transform.position.x)%width;
			int Y = (int) (Mathf.Floor(children.transform.position.y));
			children.transform.position = new Vector3(X, (Mathf.RoundToInt((Mathf.Floor(children.transform.position.y) + 0.5f)*10)) * 0.1f, children.transform.position.z);
			if(Y >= height - 1){
				obj.GetComponent<Controls>().spawn = false;
				GameOver();
				return;
			}
			grid[X, Y] = children;
			if(checkLine(Y))
			{
				completeLines.Add(Y);
			}
			Debug.Log($"Raw Y: {children.position.y}");
			Debug.Log($"Computed Y: {Y}");
		}
		if (completeLines.Count != 0)
		{
			int total = 0;
			int multiplier = 0;
			foreach(int row in completeLines)
			{
				multiplier++;
				total += multiplier;
				for(int X = 0; X < width; X++)
				{
					Destroy(grid[X,row].gameObject);
					grid[X,row] = null;
				}
			}
			points += 100 * total;
			delete();
			return;
		}
	}

	private void delete()
	{
		Segment();
		Clear();

		List<Transform> segments = new List<Transform>();

		foreach (Transform obj in transform)
		{
			if (obj.childCount == 0)
			{
				Destroy(obj.gameObject);
			}
			else
			{
				obj.GetComponent<Controls>().on = false;
				segments.Add(obj);
			}
		}

		segments.Sort((a, b) =>
		{
			float ay = GetLowestY(a);
			float by = GetLowestY(b);
			return ay.CompareTo(by);
		});

		StartCoroutine(ActivateSegments(segments));
	}

	IEnumerator ActivateSegments(List<Transform> segments)
	{
		foreach (Transform t in segments)
		{
			t.GetComponent<Controls>().on = true;
			yield return null; // one frame delay
		}
	}

	float GetLowestY(Transform t)
	{
		float minY = float.MaxValue;

		foreach (Transform c in t)
		{
			if (c.position.y < minY)
				minY = c.position.y;
		}

		return minY;
	}

	void Segment()
	{
		bool[,] visited = new bool[width, height];

		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				if (grid[x, y] != null && !visited[x, y])
				{
					GameObject seg = new GameObject();
					seg.transform.SetParent(transform, true);
					seg.transform.localPosition = Vector3.zero;

					Controls ctrl = seg.AddComponent<Controls>();
					ctrl.on = false;
					ctrl.spawn = false;
					seg.tag = "tetromino";

					Queue<Vector2Int> q = new Queue<Vector2Int>();
					q.Enqueue(new Vector2Int(x, y));
					visited[x, y] = true;

					while (q.Count > 0)
					{
						Vector2Int cell = q.Dequeue();
						Transform cube = grid[cell.x, cell.y];

						if (cube == null) continue;

						cube.SetParent(seg.transform, true);

						foreach (var dir in directions)
						{
							int neighborX = (cell.x + dir.x + width) % width;
							int neighborY = cell.y + dir.y;

							if (neighborY < 0 || neighborY >= height)
								continue;

							if (visited[neighborX, neighborY])
								continue;

							if (grid[neighborX, neighborY] == null)
								continue;

							visited[neighborX, neighborY] = true;
							q.Enqueue(new Vector2Int(neighborX, neighborY));
						}
					}
				}
			}
		}
	}

	void Clear() {
		for(int y = 0; y<height; y++)
		{
			for(int x = 0; x<width; x++)
			{
				grid[x,y] = null;
			}
		}
	}

	bool checkLine(int Y) {
		for(int i = 0; i < width; i++)
		{
			if (grid[i, Y] == null)
			{
				return false;
			}
		}
		return true;
	}

	public void GameOver() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		/* speed = 1;
		time = 0;
		Time.timeScale = 1;
		points = 0;
		foreach (Transform obj in transform)
		{
			Destroy(obj.gameObject);
		}
		Destroy(GameObject.FindGameObjectWithTag("spawner"));
		Destroy(GameObject.FindGameObjectWithTag("platform"));
		gameObject.SetActive(false); */
	}
}