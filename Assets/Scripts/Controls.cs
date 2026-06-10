using UnityEngine;

public class Controls : MonoBehaviour
{
    enum InputAction { None, Left, Right, Down, Rotate }
    InputAction action = InputAction.None;

    private Spawner spawner;
    private Tetro tetro;
    public float fallTime = 0.1f;
    private float time = 0.0f;
	public bool on = true;
	public bool spawn = true;
	private Vector3 rotationPoint = new Vector3(0.5f,0.5f,0);
	private static int width = 14;
	private Vector2 startpos;

    void Start(){
        spawner = GameObject.FindGameObjectWithTag("spawner").GetComponent<Spawner>();
        tetro = transform.parent.GetComponent<Tetro>();
        if (!isValid())
        {
            tetro.GameOver();
        }
    }

    private void HandleTouch()
	{
		if (Input.touchCount == 0)
        {
            return;
        }

		Touch touch = Input.GetTouch(0);

		if (touch.phase == TouchPhase.Began)
		{
			startpos = touch.position;
		}
		else if (touch.phase == TouchPhase.Ended)
		{
			float deltaY = touch.position.y - startpos.y;
			float normalizedX = touch.position.x / Screen.width;

			if (normalizedX > 0.7f)
				action = InputAction.Right;
			else if (normalizedX < 0.3f)
				action = InputAction.Left;
			else if (deltaY < -50)
				action = InputAction.Down;
			else
				action = InputAction.Rotate;
		}
	}

	private void HandleMouse()
	{
		if (!Input.GetMouseButtonDown(0))
        {
            return;
        }
		Vector2 pos = Input.mousePosition;

		float normalizedX = pos.x / Screen.width;

		if (normalizedX > 0.7f)
			action = InputAction.Right;
		else if (normalizedX < 0.3f)
			action = InputAction.Left;
		else
			action = InputAction.Rotate;
	}

	private void HandleKeyboard()
	{
		if (Input.GetKeyDown(KeyCode.LeftArrow))
			action = InputAction.Left;

		if (Input.GetKeyDown(KeyCode.RightArrow))
			action = InputAction.Right;

        if (Input.GetKeyDown(KeyCode.DownArrow))
			action = InputAction.Down;

		if (Input.GetKeyDown(KeyCode.Space))
			action = InputAction.Rotate;
	}

    void ExecuteAction()
    {
        if (!spawn) return;

        switch (action)
        {
            case InputAction.Left:
                MoveLeft();
                break;
            case InputAction.Right:
                MoveRight();
                break;
            case InputAction.Rotate:
                RotatePiece();
                break;
            case InputAction.Down:
                fallTime = 0.005f;
                break;
        }

        action = InputAction.None;
    }

    void MoveLeft(){
        transform.position += new Vector3(-1,0,0);
        if(!isValid())
        {
            transform.position += new Vector3(1,0,0);
        }
        else
        {
            spawner.transform.position += new Vector3(-1,0,0);
            Camera.main.transform.RotateAround(new Vector3(7,0,0), new Vector3(0,1,0), 6.2831852f/14 * Mathf.Rad2Deg);
		}
    }

    void MoveRight(){
        transform.position += new Vector3(1,0,0);
        if(!isValid())
        {
            transform.position += new Vector3(-1,0,0);
        }
        else
        {
            spawner.transform.position += new Vector3(1,0,0);
            Camera.main.transform.RotateAround(new Vector3(7,0,0), new Vector3(0,1,0), -6.2831852f/14 * Mathf.Rad2Deg);
        }
    }

    void RotatePiece(){
        transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0,0,1), -90);
        if(!isValid())
        {
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0,0,1), 90);
        }
    }

    void Wrap() {
		if(transform.position.x > width - 1 || transform.position.x < 0)
		{
			float X = (width+transform.position.x)%width;
			transform.position = new Vector3(X, transform.position.y, transform.position.z);
		}
	}

	bool isValid() {
		foreach (Transform children in transform)
		{
			if (children.transform.position.y < 0.5f)
			{
				return false;
			}
			int X = Mathf.RoundToInt(width+children.transform.position.x)%width;
			int Y = (int)(Mathf.RoundToInt((children.transform.position.y - 0.5f)*10) * 0.1f);
			if(tetro.grid[X, Y] != null)
			{
				return false;
			}
		}
		return true;
	}

	void Update ()
    {
        if(on)
        {
            if (action == InputAction.None) HandleTouch();
            if (action == InputAction.None) HandleMouse();
            if (action == InputAction.None) HandleKeyboard();
            ExecuteAction();

            time += Time.deltaTime;

            if(time > fallTime)
                {
                    transform.position = new Vector3(transform.position.x, Mathf.RoundToInt((transform.position.y - 0.1f)*10) * 0.1f,transform.position.z);
                    if(!isValid())
                    {
                        fallTime = 0.1f;
                        transform.position += new Vector3(0, 0.15f,0);
                        on = false;
                        tetro.AddToGrid(this.transform);
                        if(spawn)
                        {
                            spawn = false;
                            spawner.SpawnRandom();
                        }
                    }
                    time = 0.0f;
                }
            Wrap();
        }
    }
}