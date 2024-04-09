using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
public class GemsManager : MonoBehaviour
{
    private int _nextGem;
    private GameObject _currentGem;
    private Vector2 _tapPos;
    [SerializeField]
    private Transform _spawnPoint;
    public float _cooldown;
    private float _cd;
    // Start is called before the first frame update
    void Start()
    {
        _nextGem = Random.Range(0, 4);
    }

    // Update is called once per frame
    void Update()
    {
        if(_cd > 0)_cd -= Time.deltaTime;
    }

    [Button]
    private void NextGem()
    {
        _cd = _cooldown;
        _nextGem = Random.Range(0, 4);
        Debug.Log("Next gem is : " + _nextGem);
    }


    public void OnTouchDrag(InputAction.CallbackContext ctx)
    {
        if (_cd <= 0)
        {
            _tapPos = Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>());
            if (ctx.started)
            {
                //Debug.Log("INSTANTIATE");
                _currentGem = Instantiate(GameManager.instance.AllGems[_nextGem], new Vector2(_tapPos.x, _spawnPoint.position.y), GameManager.instance.AllGems[_nextGem].transform.rotation);
                if (_currentGem == null) return;
                //_currentGem.GetComponent<Rigidbody2D>().gravityScale = 0;
                _currentGem.GetComponent<Rigidbody>().useGravity = false;
            }
            else if (ctx.performed) 
            {
                if (_currentGem == null) return;
                _tapPos = Camera.main.ScreenToWorldPoint(new Vector3(ctx.ReadValue<Vector2>().x, 0f, 10f));
                _tapPos.y = _spawnPoint.position.y;
                //Debug.Log(tapPos);
                _currentGem.transform.position = new Vector2(_tapPos.x, _tapPos.y);
            }
            else if(ctx.canceled)
            {
                if (_currentGem == null) return;
                Debug.Log("DROP");
                _currentGem.GetComponent<Rigidbody>().useGravity = true;
                _currentGem = null;
                NextGem();
            }
        }
    }

    private void DragGem()
    {

    }
}
