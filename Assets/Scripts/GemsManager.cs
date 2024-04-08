using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
public class GemsManager : MonoBehaviour
{
    public GameManager _gameManager;
    private int _nextGem;
    private GameObject _currentGem;
    private Vector2 _tapPos;
    [SerializeField]
    private Transform _spawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        NextGem();
    }

    // Update is called once per frame
    void Update()
    {
        //Vector2 mousePos = Camera.main.ScreenToWorldPoint();
    }

    [Button]
    private void NextGem()
    {
        _nextGem = Random.Range(0, _gameManager.AllGems.Length);
        Debug.Log("Next gem is : " + _nextGem);
    }


    public void OnTouchDrag(InputAction.CallbackContext ctx)
    {

        _tapPos = Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>());
        if (ctx.started)
        {
            //Debug.Log("INSTANTIATE");
            _currentGem = Instantiate(_gameManager.AllGems[_nextGem], new Vector2(_tapPos.x, _spawnPoint.position.y), Quaternion.identity);
            _currentGem.GetComponent<Rigidbody2D>().gravityScale = 0;
        }
        else if (ctx.performed) 
        {
            _tapPos = Camera.main.ScreenToWorldPoint(new Vector3(ctx.ReadValue<Vector2>().x, 0f, 10f));
            _tapPos.y = _spawnPoint.position.y;
            //Debug.Log(tapPos);
            _currentGem.transform.position = new Vector2(_tapPos.x, _tapPos.y);
        }
        else if(ctx.canceled)
        {
            Debug.Log("DROP");
            _currentGem.GetComponent<Rigidbody2D>().gravityScale = 1;
            NextGem();
        }
    }

    private void DragGem()
    {

    }
}
